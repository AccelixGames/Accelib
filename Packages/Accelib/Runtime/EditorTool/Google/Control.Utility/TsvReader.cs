using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Accelib.EditorTool.Google.Control.Utility
{
    public static class TsvReader
    {
        // TSV uses a tab character ('\t') as a delimiter.
        // The regex pattern is simpler than CSV since tabs are less ambiguous.
        private const string SplitRe = @"\t"; 
        private const string LineSplitRe = @"\r\n|\n\r|\n|\r";

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

                // Unlike CSV, TSV files often don't use complex quoting rules for fields containing tabs.
                // We'll process each line as a single row.
                foreach (var line in lines)
                {
                    // Skip empty lines, unless a field is explicitly an empty string.
                    if (string.IsNullOrEmpty(line.Trim()))
                    {
                        // To handle rows that are entirely empty, we can choose to add an empty row or skip.
                        // For this implementation, we will skip fully empty lines.
                        // If you need to preserve empty rows, remove this check.
                        continue;
                    }

                    // Split the line by the tab delimiter.
                    var values = Regex.Split(line, SplitRe);
                    var row = new List<string>();

                    for (var j = 0; j < values.Length; j++)
                    {
                        var value = values[j];

                        // TSV files may use quotes, but they are less common than in CSV.
                        // A simple trim and un-quote logic can be applied, if needed.
                        // Example: if a value is ""hello"", trim the quotes.
                        if (value.StartsWith("\"") && value.EndsWith("\""))
                        {
                            value = value.Substring(1, value.Length - 2);
                        }
                        
                        // Handle potential escaped quotes if your TSV data uses them
                        // (e.g., replace "" with ")
                        value = value.Replace("\"\"", "\"");

                        row.Add(value);
                    }

                    result.Add(row);
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