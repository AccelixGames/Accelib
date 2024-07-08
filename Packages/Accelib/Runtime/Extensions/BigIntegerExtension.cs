using System.Numerics;

namespace Accelib.Extensions
{
    public static class BigIntegerExtension
    {
        private const long ConversionRate = 1000;
        private static readonly string[] Units = { "", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

        public static string ToAlphabetString(this string target)
        {
            if (!BigInteger.TryParse(target, out var result))
                return "0";

            return ToAlphabetString(result);
        }
        
        public static string ToAlphabetString(this BigInteger bi)
        {
            var unitIndex = 0;
 
            while (bi.CompareTo(ConversionRate) >= 0 && unitIndex < Units.Length - 1)
            {
                bi /= ConversionRate;
                unitIndex++;
            }
 
            // 셋 째 자리까지만 표기
            var formattedAmount = $"{bi:F3}";
 
            // 소수점 끝이 0이라면 안보이게
            formattedAmount = formattedAmount.TrimEnd('0').TrimEnd('.');
 
            // 값이 처리 후 비어 있다면, 그 값을 '0'으로 설정합니다.
            if (string.IsNullOrEmpty(formattedAmount))
            {
                formattedAmount = "0";
            }
 
            return $"{formattedAmount}{Units[unitIndex]}";
        }
    }
}