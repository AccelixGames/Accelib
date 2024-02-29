using UnityEngine;

namespace Accelib.Extensions
{
    public static class VectorExtension
    {
        public static Vector3Int ToVec3(this in Vector2Int v) => new Vector3Int(v.x, v.y, 0);
        
        public static Vector3 ToVec3(this in Vector2 v) => new Vector3(v.x, v.y, 0f);
        
        public static Vector2 ToCardinal(this in Vector2 v, float minMagnitude = 0.01f)
        {
            if (v.sqrMagnitude <= minMagnitude) return Vector2.zero;
            
            var u = Vector2.Dot(v, Vector2.up);
            var d = Vector2.Dot(v, Vector2.down);
            var l = Vector2.Dot(v, Vector2.left);
            var r = Vector2.Dot(v, Vector2.right);

            var max = u;
            var result = Vector2.up;
            
            if (d > max)
            {
                max = d;
                result = Vector2.down;
            }
            if (l > max)
            {
                max = l;
                result = Vector2.left;
            }
            if (r > max)
            {
                max = r;
                result = Vector2.right;
            }

            return result;
        }

        public static float Random(this in Vector2 v) => UnityEngine.Random.Range(v.x, v.y);
    }
}