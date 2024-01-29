using UnityEngine;

namespace Accelib.Extensions
{
    public static class RectExtension
    {
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