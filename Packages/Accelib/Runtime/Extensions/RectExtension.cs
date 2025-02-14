using UnityEngine;

namespace Accelib.Extensions
{
    public static class RectExtension
    {
        public static Vector2 Random(this Rect rect)
        {
            var x = UnityEngine.Random.Range(rect.xMin, rect.xMax);
            var y = UnityEngine.Random.Range(rect.yMin, rect.yMax);
            return new Vector2(x, y);
        } 

        public static Vector2 Repeat(this Rect rect, Vector2 value)
        {
            if (value.x > rect.xMax)
                value.x -= rect.width;
            else if (value.x < rect.xMin)
                value.x += rect.width;
            
            if (value.y > rect.yMax)
                value.y -= rect.height;
            else if (value.y < rect.yMin)
                value.y += rect.height;

            return value;
        } 
        
        public static bool IsInsideInclusive(this RectInt rect, Vector2Int vec)
        {
            if (rect.xMin > vec.x) return false;
            if (rect.xMax < vec.x) return false;
            if (rect.yMin > vec.y) return false;
            if (rect.yMax < vec.y) return false;

            return true;
        } 
        
        public static bool IsInsideExclusive(this RectInt rect, Vector2Int vec)
        {
            if (rect.xMin >= vec.x) return false;
            if (rect.xMax <= vec.x) return false;
            if (rect.yMin >= vec.y) return false;
            if (rect.yMax <= vec.y) return false;

            return true;
        } 
    }
}