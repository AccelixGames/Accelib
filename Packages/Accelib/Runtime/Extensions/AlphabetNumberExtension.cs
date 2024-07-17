using System;
using System.Numerics;
using UnityEngine;

namespace Accelib.Extensions
{
    public static class AlphabetNumberExtension
    {
        private const double ConversionRate =  1000D;
        private const double ConversionRateR =  1D / ConversionRate;
        private static readonly string[] Units = 
        {
            "", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", 
            "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", 
            "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", 
            "ca", "cb", "cc", "cd", "ce", "cf", "cg", "ch", "ci", "cj", "ck", "cl", "cm", "cn", "co", "cp", "cq", "cr", "cs", "ct", "cu", "cv", "cw", "cx", "cy", "cz"
        };

        public static string ToAlphabetString(this double target)
        {
            if (double.IsInfinity(target))
                return "Infinity";
            
            var unitIndex = 0;
 
            while (target.CompareTo(ConversionRate) >= 0 && unitIndex < Units.Length - 1)
            {
                target *= ConversionRateR;
                unitIndex++;
            }
 
            // 첫째 자리까지만 표기
            var formattedAmount = ""; 
            formattedAmount = unitIndex > 0 ? $"{(int)Math.Truncate(target * 10d) / 10f}" : $"{(int)target}";
 
            // 소수점 끝이 0이라면 안보이게
            // if (unitIndex <= 0)
            // {
            //     formattedAmount = formattedAmount.TrimEnd('0').TrimEnd('.');
            // }
 
            // 값이 처리 후 비어 있다면, 그 값을 '0'으로 설정합니다.
            if (string.IsNullOrEmpty(formattedAmount)) 
                formattedAmount = "0";
 
            return $"{formattedAmount}{Units[unitIndex]}";
        }
    }
}