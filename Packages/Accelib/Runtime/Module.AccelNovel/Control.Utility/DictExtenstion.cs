using System;
using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Control.Utility
{
    public static class DictExtenstion
    {
        public static T GetEnum<T>(this Dictionary<string, string> dict, string key, T defaultValue = default)
            where T : struct, Enum
        {
            if (dict?.TryGetValue(key, out var str) ?? false)
                if (Enum.TryParse(str, true, out T value))
                    return value;

            return defaultValue;
        }
        
        public static Color GetColor(this Dictionary<string, string> dict, string key, Color defaultValue)
        {
            var colorCode = dict?.GetValueOrDefault(key, null);
            if (string.IsNullOrEmpty(colorCode) || !ColorUtility.TryParseHtmlString(colorCode, out var color)) 
                return defaultValue;

            return color;
        }

        public static string GetString(this Dictionary<string, string> dict, string key, string defaultValue)
        {
            return dict?.GetValueOrDefault(key, defaultValue) ?? defaultValue;
        }
        
        public static bool GetBool(this Dictionary<string, string> dict, string key, bool defaultValue)
        {
            if(dict?.TryGetValue(key, out var str) ?? false)
                if (bool.TryParse(str, out var value))
                    return value;

            return defaultValue;
        }
        
        public static int GetInt(this Dictionary<string, string> dict, string key, int defaultValue)
        {
            if(dict?.TryGetValue(key, out var str) ?? false)
                if (int.TryParse(str, out var value))
                    return value;

            return defaultValue;
        }
        
        public static float GetFloat(this Dictionary<string, string> dict, string key, float defaultValue)
        {
            if(dict?.TryGetValue(key, out var str) ?? false)
                if (float.TryParse(str, out var value))
                    return value;

            return defaultValue;
        }
    }
}