using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Logging;
using TMPro;

namespace Accelib.Extensions
{
    public static class StringExtension
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            var index = text.IndexOf(search, StringComparison.Ordinal);
            if (index < 0) return text;
            
            return text[..index] + replace + text[(index + search.Length)..];
        }
        
        public static TMP_Text Omit(this TMP_Text target, int maxLength)
        {
            var text = target.text;
            var length = text.Length;

            if (length > maxLength)
                target.text = text.Remove(maxLength, length -maxLength).Insert(maxLength, "...");

            return target;
        }
        
        public static string Omit(this string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            
            var length = text.Length;

            if (length > maxLength)
                text = text.Remove(maxLength, length -maxLength).Insert(maxLength, "..");

            return text;
        }

        /// <summary>
        /// 작성자 : 강지현
        /// 확장자 추출
        /// 마지막 "."을 기준으로 오른쪽 문자열을 확장자로 가짐
        /// </summary>
        /// <param name="text">입력받을 문자열</param>
        /// <returns>추출한 확장자</returns>
        public static string GetExtension(this string text)
        {
            try
            {
                var arr = text.Split('.');
                if (arr.Length < 1)
                    throw new Exception("확장자가 없습니다.");
                
                return arr[^1].ToLower();
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                return "";
            }
        }

        /// <summary>
        /// 작성자 : 강지현
        /// 슬래쉬(/)문자 인코딩 함수
        /// </summary>
        /// <param name="url"></param> 대상 url
        /// <returns>url 중 슬래쉬(/)를 %2F로 치환</returns>
        public static string GetSlashEncoding(this string url)
        {
            return url.Replace("/", "%2F");
        }
        
        /// <summary>
        /// 작성자 : 강지현
        /// 플러스(+) 문자 인코딩 함수
        /// </summary>
        /// <param name="url"></param>
        /// <returns>url 중 플러스(+)를 %2B로 치환</returns>
        public static string GetPlusEncoding(this string url)
        {
            return url.Replace("+", "%2B");
        }
        /// <summary>
        /// 문자열의 첫 문자를 대문자로 변환하는 함수
        /// </summary>
        /// <param name="text">변환할 문자열</param>
        /// <returns>첫 문자를 대문자로 변환한 문자열</returns>
        public static string Capitalize(this string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) 
                    throw new Exception("문자열이 비어있습니다.");
                
                return text[0].ToString().ToUpper() + text.Substring(1);
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                return "";
            }
        }


        private const string Symbols = "KMGT";

        /// <summary>
        /// 숫자(문자열)를 KMGT 단위로 변환하는 함수
        /// 1000 -> 1K, 123456 -> 123.4K
        /// </summary>
        /// <param name="text">변환할 문자열</param>
        /// <returns>단위 변환해서 소숫점 한자리까지 나타낸 문자열</returns>
        /// <exception cref="Exception"></exception>
        public static string GetNumberSymbol(this string text)
        {
            try
            {
                // 입력받은 문자열이 없을 경우
                if (string.IsNullOrEmpty(text)) 
                    throw new Exception("문자열이 비어있습니다.");
                
                // 앞에 0 제거 ex) 00123 -> 123
                var strNum = text.TrimStart('0');
                
                // 0만 입력된 경우
                if (string.IsNullOrEmpty(strNum))
                    return "0";
                
                // 0이 아닌 수가 입력된 경우
                var symbolIdx = (strNum.Length - 1) / 3 < Symbols.Length ? (strNum.Length - 1) / 3 : Symbols.Length;
                var strLen = strNum.Length - symbolIdx * 3;
                
                if (symbolIdx < 1)
                    return $"{strNum}";

                // 소숫점 뒤가 0일 경우 소숫점과 0을 출력하지 않음
                if (strNum[^(symbolIdx * 3)].Equals('0'))
                    return $"{strNum.Substring(0,strLen)}{Symbols[symbolIdx - 1].ToString()}";
                
                return $"{strNum.Substring(0,strLen)}.{strNum[strLen]}{Symbols[symbolIdx - 1].ToString()}";
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                return "";
            }
        }
        
        public static string NullCheckString(this string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    throw new Exception("문자열이 비어있습니다.");
                
                return text;
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                return "";
            }
        }
        
        public static string FormatComma(this string text)
        {
            var length = text.Length;
            if (length <= 3) return text;
            
            var result = text[..(length % 3)];
            for (var i = length % 3; i < length; i += 3)
            {
                if (i > 0) result += ","; // 콤마 추가
                result += text.Substring(i, 3);
            }

            return result;
        }
    }
}