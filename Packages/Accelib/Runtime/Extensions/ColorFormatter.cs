using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Accelib.Extensions
{
    public class ColorConverter : JsonConverter
    {
        public ColorConverter() { }
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (!ColorUtility.TryParseHtmlString("#" + reader.Value, out var loadedColor))
                    loadedColor = Color.white;
                return loadedColor;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse color {objectType} : {ex.Message}");
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var color = value == null ? Color.white : (Color)value;
            
            var val = ColorUtility.ToHtmlStringRGB(color);
            writer.WriteValue(val);
        }
    }
}