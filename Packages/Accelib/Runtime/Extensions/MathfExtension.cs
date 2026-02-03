using UnityEngine;

namespace Accelib.Extensions
{
    public static class MathfExtension
    {
        public static float Step(this float value, float roundTo = 1f) => 
            Mathf.Round(value / roundTo) * roundTo;
        
        public static Vector2 Step(this Vector2 value, float roundTo = 1f)
        {
            value.x = Mathf.Round(value.x / roundTo) * roundTo;
            value.y = Mathf.Round(value.y / roundTo) * roundTo;

            return value;
        }
        
        public static Vector2 Step(this Vector2 value, Vector2 roundTo)
        {
            value.x = Mathf.Round(value.x / roundTo.x) * roundTo.x;
            value.y = Mathf.Round(value.y / roundTo.y) * roundTo.y;

            return value;
        }
        
        public static Vector3 Step(this Vector3 value, float roundTo)
        {
            value.x = Mathf.Round(value.x / roundTo) * roundTo;
            value.y = Mathf.Round(value.y / roundTo) * roundTo;
            value.z = Mathf.Round(value.z / roundTo) * roundTo;

            return value;
        }
    }
}