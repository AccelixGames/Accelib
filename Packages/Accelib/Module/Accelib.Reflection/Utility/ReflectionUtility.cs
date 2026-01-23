#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Accelib.Reflection.Model;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Accelib.Reflection.Utility
{
    public static class ReflectionUtility
    {
        private const int MaxDepth = 6;
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        
        public static IEnumerable<string> GetMemberList(Object target)
        {
            if (target == null) yield break;

            var rootType = target.GetType();
            foreach (var c in ScanType(rootType, "", 0))
                yield return c;
        }

        // public static (Object, FieldInfo) GetFieldInfo(Object target, string memberPath)
        // {
        //     if (target == null || string.IsNullOrEmpty(memberPath))
        //         return null;
        //
        //     var type = target.GetType();
        //     FieldInfo field = null;
        //
        //     // "a.b.c" → ["a","b","c"]
        //     foreach (var name in memberPath.Split('.'))
        //     {
        //         // 현재 타입에서 필드 탐색
        //         field = type.GetField(name, Flags);
        //
        //         // 경로가 깨졌을 경우
        //         if (field == null) return null;
        //
        //         // 다음 depth로 타입 이동
        //         type = field.FieldType;
        //     }
        //
        //     // 마지막에 도달한 FieldInfo 반환
        //     return field;
        // }

        public static object GetMemberValue(Object target, string memberPath)
        {
            // target 없거나 경로 비어있으면 탐색 불가함
            if (target == null || string.IsNullOrEmpty(memberPath))
                return null;

            // 현재 접근 중인 객체 인스턴스임
            object obj = target;

            // 현재 타입 정보임
            var type = target.GetType();

            // 점(.) 기준으로 경로 분해해서 순차 접근함
            foreach (var name in memberPath.Split('.'))
            {
                // 먼저 Field 탐색함
                var field = type.GetField(name, Flags);
                if (field != null)
                {
                    // Field 주인 값 가져와서 업데이트
                    obj = field.GetValue(obj);

                    // 중간 값이 null이면 더 이상 접근 불가함
                    if (obj == null) return null;

                    // 다음 단계에서 사용할 타입 갱신함
                    type = field.FieldType;
                    continue;
                }

                // Field 없으면 Property 탐색함
                var property = type.GetProperty(name, Flags);

                // Property 없거나 getter 없으면 실패 처리함
                if (property == null || !property.CanRead)
                    return 0;

                // Property 값 가져옴
                obj = property.GetValue(obj);

                // 중간 값이 null이면 더 이상 접근 불가함
                if (obj == null)
                    return 0;

                // 다음 단계에서 사용할 타입 갱신함
                type = property.PropertyType;
            }

            // 최종 값 반환함
            return obj;
        }
        
        public static double ConvertToDouble(object value)
        {
            if (value == null) return 0.0;

            try
            {
                // Fast path for common types
                switch (value)
                {
                    case int v: return v;
                    case long v: return v;
                    case float v: return v;
                    case double v: return v;
                    case decimal v: return (double)v;
                    case uint v: return v;
                    case ulong v: return v;
                    case short v: return v;
                    case ushort v: return v;
                    case byte v: return v;
                    case sbyte v: return v;
                    case bool v: return v ? 0d : 1d;
                    default: return Convert.ToDouble(value);
                }
            }
            catch
            {
                return 0d;
            }
        }
        
        private static IEnumerable<string> ScanType(Type type, string prefix, int depth)
        {
            if (depth > MaxDepth) yield break;

            foreach (var fieldInfo in type.GetFields(Flags))
            {
                var fieldType = fieldInfo.FieldType;
                var path = string.IsNullOrEmpty(prefix) ? fieldInfo.Name : $"{prefix}.{fieldInfo.Name}";

                if (TryMapNumeric(fieldType, out var nt))
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
        }
        
        private static bool ShouldRecurse(Type t)
        {
            if (t.IsPrimitive || t.IsEnum) return false;
            if (t == typeof(string)) return false;
            if (typeof(UnityEngine.Object).IsAssignableFrom(t)) return false;

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

            numericType = default;
            return false;
        }
    }
}
#endif