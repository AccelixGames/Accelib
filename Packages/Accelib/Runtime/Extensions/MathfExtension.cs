using UnityEngine;

namespace Accelib.Extensions
{
    public static class MathfExtension
    {
        public static float RoundTo(this float value, float roundTo = 1f) => 
            Mathf.Round(value / roundTo) * roundTo;
        
        public static Vector2 RoundTo(this Vector2 value, float roundTo = 1f)
        {
            value.x = Mathf.Round(value.x / roundTo) * roundTo;
            value.y = Mathf.Round(value.y / roundTo) * roundTo;

            return value;
        }
        
        public static Vector2 RoundTo(this Vector2 value, Vector2 roundTo)
        {
            value.x = Mathf.Round(value.x / roundTo.x) * roundTo.x;
            value.y = Mathf.Round(value.y / roundTo.y) * roundTo.y;

            return value;
        }
    }
}