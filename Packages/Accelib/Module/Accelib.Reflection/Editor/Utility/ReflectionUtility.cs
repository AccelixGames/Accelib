#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Accelib.Reflection.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Accelib.Reflection.Utility
{
    public static class ReflectionUtility
    {
        private const int MaxDepth = 6;
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public;

        public static IEnumerable<string> GetMemberList(Object target)
        {
            if (target == null) yield break;

            var rootType = target.GetType();
            foreach (var c in ScanType(rootType, "", 0))
                yield return c;
        }

        private static IEnumerable<string> ScanType(Type type, string prefix, int depth)
        {
            if (depth > MaxDepth) yield break;

            // 1) Fields
            foreach (var fieldInfo in type.GetFields(Flags))
            {
                var fieldType = fieldInfo.FieldType;
                var path = string.IsNullOrEmpty(prefix) ? fieldInfo.Name : $"{prefix}.{fieldInfo.Name}";

                if (TryMapNumeric(fieldType, out var nt))
                {
                    yield return path;
                    continue;
                }

                // ReactiveProperty<T> 계열 필드 → .CurrentValue 경로 추가
                // ReadOnlyReactiveProperty<T>에는 Value가 없고 CurrentValue만 존재함
                if (TryGetReactivePropertyValueType(fieldType, out var rpValueType))
                {
                    var valuePath = $"{path}.CurrentValue";
                    if (TryMapNumeric(rpValueType, out _))
                        yield return valuePath;
                    continue;
                }

                // 문자열 컬렉션 필드 → Contains 비교용
                if (IsStringCollectionType(fieldType))
                {
                    yield return path;
                    continue;
                }

                if (ShouldRecurse(fieldType))
                {
                    foreach (var sub in ScanType(fieldType, path, depth + 1))
                        yield return sub;
                }
            }

            // 2) Properties (public/shared)
            foreach (var propInfo in type.GetProperties(Flags))
            {
                // getter 없으면 스킵
                if (!propInfo.CanRead) continue;

                // indexer는 경로 지원 없으니 스킵
                if (propInfo.GetIndexParameters().Length > 0) continue;

                // Unity Object의 흔한 heavy/side-effect property는 제외
                if (IsUnityHeavyProperty(type, propInfo)) continue;

                var propType = propInfo.PropertyType;
                var path = string.IsNullOrEmpty(prefix) ? propInfo.Name : $"{prefix}.{propInfo.Name}";

                if (TryMapNumeric(propType, out _))
                {
                    yield return path;
                    continue;
                }

                // ReactiveProperty<T> 계열 프로퍼티 → .CurrentValue 경로 추가
                // ReadOnlyReactiveProperty<T>에는 Value가 없고 CurrentValue만 존재함
                if (TryGetReactivePropertyValueType(propType, out var rpValueType2))
                {
                    var valuePath = $"{path}.CurrentValue";
                    if (TryMapNumeric(rpValueType2, out _))
                        yield return valuePath;
                    continue;
                }

                // 문자열 컬렉션 프로퍼티 → Contains 비교용
                if (IsStringCollectionType(propType))
                {
                    yield return path;
                    continue;
                }

                if (ShouldRecurse(propType))
                {
                    foreach (var sub in ScanType(propType, path, depth + 1))
                        yield return sub;
                }
            }
        }

        /// <summary>
        /// 제네릭 타입이 ReactiveProperty 계열인지 확인하고, 내부 Value 타입을 반환함.
        /// ReactiveProperty, SerializableReactiveProperty, ReadOnlyReactiveProperty 등을 지원함.
        /// R3 어셈블리 참조 없이 타입 이름 기반으로 판별함.
        /// </summary>
        private static bool TryGetReactivePropertyValueType(Type t, out Type valueType)
        {
            valueType = null;
            if (!t.IsGenericType) return false;

            var name = t.GetGenericTypeDefinition().Name;
            if (name.StartsWith("ReactiveProperty") ||
                name.StartsWith("SerializableReactiveProperty") ||
                name.StartsWith("ReadOnlyReactiveProperty") ||
                name.StartsWith("BindableReactiveProperty"))
            {
                var args = t.GetGenericArguments();
                if (args.Length == 1)
                {
                    valueType = args[0];
                    return true;
                }
            }

            return false;
        }

        private static bool ShouldRecurse(Type t)
        {
            if (t.IsPrimitive || t.IsEnum) return false;
            if (t == typeof(string)) return false;
            if (t == typeof(Color)) return false;
            if (typeof(Object).IsAssignableFrom(t)) return false;
            if (typeof(Delegate).IsAssignableFrom(t)) return false;

            // Arrays / generics (List<>) require index-path support; intentionally excluded in this drop-in.
            if (t.IsArray) return false;
            if (t.IsGenericType) return false;

            // Recurse into classes/structs
            return t.IsClass || (t.IsValueType && !t.IsPrimitive && !t.IsEnum);
        }

        private static bool TryMapNumeric(Type t, out ENumericType numericType)
        {
            if (t == typeof(sbyte)) { numericType = ENumericType.SByte; return true; }
            if (t == typeof(byte)) { numericType = ENumericType.Byte; return true; }
            if (t == typeof(short)) { numericType = ENumericType.Short; return true; }
            if (t == typeof(ushort)) { numericType = ENumericType.UShort; return true; }
            if (t == typeof(int)) { numericType = ENumericType.Int; return true; }
            if (t == typeof(uint)) { numericType = ENumericType.UInt; return true; }
            if (t == typeof(long)) { numericType = ENumericType.Long; return true; }
            if (t == typeof(ulong)) { numericType = ENumericType.ULong; return true; }
            if (t == typeof(float)) { numericType = ENumericType.Float; return true; }
            if (t == typeof(double)) { numericType = ENumericType.Double; return true; }
            if (t == typeof(decimal)) { numericType = ENumericType.Decimal; return true; }
            if (t == typeof(bool)) { numericType = ENumericType.Boolean; return true; }

            // enum 처리
            if (t.IsEnum)
            {
                numericType = ENumericType.Enum;
                return true;
            }

            // implicit 숫자 변환 연산자 보유 타입 (e.g., PriceUnit → float)
            if (HasImplicitNumericConversion(t))
            {
                numericType = ENumericType.Float;
                return true;
            }

            numericType = default;
            return false;
        }

        /// <summary> implicit 숫자 변환 연산자(op_Implicit → float/double/int/long)가 있는지 확인한다. </summary>
        private static bool HasImplicitNumericConversion(Type t)
        {
            if (t.IsPrimitive || t.IsEnum) return false;

            foreach (var m in t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (m.Name != "op_Implicit") continue;
                var rt = m.ReturnType;
                if (rt != typeof(float) && rt != typeof(double) && rt != typeof(int) && rt != typeof(long)) continue;
                var ps = m.GetParameters();
                if (ps.Length == 1 && ps[0].ParameterType == t)
                    return true;
            }

            return false;
        }

        /// <summary> 문자열 컬렉션 타입인지 확인한다. Contains 비교에 사용 가능한 타입. </summary>
        private static bool IsStringCollectionType(Type t)
        {
            return typeof(IEnumerable<string>).IsAssignableFrom(t) && t != typeof(string);
        }

        private static bool IsUnityHeavyProperty(Type ownerType, PropertyInfo prop)
        {
            // UnityEngine.Object 파생에서 흔히 비용/부작용 큰 것들
            if (typeof(Object).IsAssignableFrom(ownerType))
            {
                // Component / GameObject / Transform 계열에서 자주 문제되는 프로퍼티들
                var n = prop.Name;
                if (n == "transform" || n == "gameObject" || n == "rigidbody" || n == "camera" || n == "light")
                    return true;
            }

            return false;
        }
    }
}
#endif
