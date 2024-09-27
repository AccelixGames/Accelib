using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Accelib.Editor.GoogleSheet
{
    public static class CsvReader
    {
        private const string SplitRe = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        private const string LineSplitRe = @"\r\n|\n\r|\n|\r";
        private static readonly char[] TrimChars = { '\"' };

        public static List<List<string>> Read(string text)
        {
            try
            {
                var result = new List<List<string>>();
                if (string.IsNullOrEmpty(text)) 
                    return null;

                var lines = Regex.Split(text, LineSplitRe);
                if (lines.Length <= 0) 
                    return null;

                bool insideQuotes = false; // 따옴표 안에 있는지 추적
                string currentLine = string.Empty;

                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    // if (string.IsNullOrEmpty(line)) continue;

                    // 따옴표로 열리는 필드가 계속될 경우 현재 줄에 이어붙임
                    currentLine += (insideQuotes ? "\n" : "") + line;

                    // 따옴표가 짝이 맞는지 확인 (짝이 맞지 않으면 줄바꿈된 데이터로 간주)
                    int quoteCount = 0;
                    foreach (char c in currentLine)
                    {
                        if (c == '\"')
                            quoteCount++;
                    }

                    // 따옴표가 짝이 맞으면 한 행으로 간주, 아니면 다음 줄과 합쳐 계속 처리
                    if (quoteCount % 2 == 0)
                    {
                        insideQuotes = false; // 따옴표가 닫힘, 한 줄로 처리 가능

                        // 쉼표로 필드 분리 및 트림 처리
                        var values = Regex.Split(currentLine, SplitRe);
                        var row = new List<string>();

                        for (var j = 0; j < values.Length; j++)
                        {
                            var value = values[j].Trim();

                            // CSV에서 필드가 ""로 감싸져 있으면 그 안의 ""를 제거
                            if (value.StartsWith("\"") && value.EndsWith("\""))
                            {
                                value = value.Substring(1, value.Length - 2).Replace("\"\"", "\"");
                            }

                            row.Add(value);
                        }

                        result.Add(row);
                        currentLine = string.Empty; // 처리된 후 초기화
                    }
                    else
                    {
                        insideQuotes = true; // 따옴표가 아직 닫히지 않음, 다음 줄로 이어감
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }
    }
}
