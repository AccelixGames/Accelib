using System;
using System.Reflection;
using Accelib.Reflection.Model;
using UnityEngine;

namespace Accelib.Reflection.Utility
{
    public static class CachedReflectionUtility
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// memberPath를 기반으로 Field/Property 체인을 생성해서 반환함
        /// - 이 함수가 “비싼 구간”임(리플렉션 탐색)
        /// - 런타임에서는 1회만 호출하고 결과를 캐싱해서 쓰는 방식 권장함
        /// </summary>
        public static CachedChain BuildChain(UnityEngine.Object target, string memberPath)
        {
            if (!target || string.IsNullOrEmpty(memberPath))
                return null;

            var names = memberPath.Split('.');
            var chain = new MemberInfo[names.Length];

            var type = target.GetType();
            for (var i = 0; i < names.Length; i++)
            {
                var name = names[i];

                // Field 우선 탐색함
                var field = type.GetField(name, Flags);
                if (field != null)
                {
                    chain[i] = field;
                    type = field.FieldType;
                    continue;
                }

                // Field 없으면 Property 탐색함
                var prop = type.GetProperty(name, Flags);
                // getter가 없는 property는 읽을 수 없으므로 실패 처리함
                if (prop == null || prop.GetGetMethod(true) == null)
                    return null;

                chain[i] = prop;
                type = prop.PropertyType;
            }

            return new CachedChain
            {
                Chain = chain,
                FinalType = type
            };
        }

        /// <summary>
        /// “결과 타입 고정(double)” 버전 getter를 생성해서 반환함
        /// - 반환된 Func는 target을 캡처함 (target이 ScriptableObject인 케이스에 적합)
        /// - 체인 탐색을 매번 하지 않고, 체인 순회 + double 변환만 수행함
        /// </summary>
        public static Func<double> BuildDoubleGetter(UnityEngine.Object target, CachedChain cached)
        {
            if (!target || cached is not { IsValid: true })
                return null;

            // FinalType이 숫자 계열인지 간단 검증함 (필요 없으면 제거 가능)
            // 숫자가 아니어도 ConvertToDouble에서 0으로 떨어지게 할 수 있음
            return () =>
            {
                object obj = target;

                foreach (var m in cached.Chain)
                {
                    if (obj == null) return 0.0;

                    if (m is FieldInfo fi) obj = fi.GetValue(obj);
                    else if (m is PropertyInfo pi) obj = pi.GetValue(obj);
                    else return 0.0;
                }

                return ConvertToDoubleFast(obj);
            };
        }

        /// <summary>
        /// object 값을 double로 변환함
        /// - 가장 흔한 numeric 타입은 switch 패턴으로 빠르게 처리함
        /// - 변환 실패 시 0 반환함
        /// </summary>
        public static double ConvertToDoubleFast(object value)
        {
            if (value == null) return 0.0;

            try
            {
                // 자주 쓰는 값 타입 빠른 경로임
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

                    // bool 등을 섞어 쓰는 경우가 있으면 필요에 따라 추가 가능함
                    // case bool v: return v ? 1.0 : 0.0;

                    default:
                        // 그 외 타입은 Convert에 위임함 (비싸지만 예외 케이스임)
                        return Convert.ToDouble(value);
                }
            }
            catch
            {
                return 0.0;
            }
        }
        
        
        // /// <summary>
        // /// 캐싱된 체인을 사용해서 최종 값을 object로 반환함
        // /// - 체인 순회 중간에 null 나오면 실패로 null 반환함
        // /// </summary>
        // public static object GetValue(UnityEngine.Object target, CachedChain cached)
        // {
        //     if (target == null || cached == null || !cached.IsValid)
        //         return null;
        //
        //     object obj = target;
        //
        //     // 체인을 따라가며 값을 내려받음
        //     foreach (var m in cached.Chain)
        //     {
        //         if (obj == null) return null;
        //
        //         if (m is FieldInfo fi)
        //         {
        //             obj = fi.GetValue(obj);
        //         }
        //         else if (m is PropertyInfo pi)
        //         {
        //             obj = pi.GetValue(obj);
        //         }
        //         else
        //         {
        //             return null;
        //         }
        //     }
        //
        //     return obj;
        // }
    }
}