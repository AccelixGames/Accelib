using UnityEngine;

namespace Accelib.Extensions
{
    public static class VectorExtension
    {
        public static int CompareTo(this Vector2Int a, Vector2Int b)
        {
            var compY = a.y.CompareTo(b.y);
            return compY == 0 ? a.x.CompareTo(b.x) : compY;
        }
        
        public static Vector3 Clamp(this Vector3 v, int min, int max)
        {
            v.x = Mathf.Clamp(v.x, min, max);
            v.y = Mathf.Clamp(v.y, min, max);
            return v;
        }
        
        public static Vector2Int Clamp(this Vector2Int v, int min, int max)
        {
            v.x = Mathf.Clamp(v.x, min, max);
            v.y = Mathf.Clamp(v.y, min, max);
            return v;
        }

        public static Vector2Int ToVec2Int(this in Vector3 v) => new((int)v.x, (int)v.y);
        public static Vector2Int ToVec2IntRound(this in Vector3 v) => new(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));

        public static Vector2Int ToVec2Int(this in Vector2 v) => new((int)v.x, (int)v.y);
        public static Vector2Int ToVec2IntRound(this in Vector2 v) => new(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));

        public static Vector2Int Sign(this in Vector2Int v) => new(
            v.x == 0 ? 0 : v.x > 0 ? 1 : -1, 
            v.y == 0 ? 0 : (v.y > 0 ? 1 : -1));

        public static Vector3Int ToVec3Int(this in Vector2Int v) => new(v.x, v.y, 0);
        public static Vector3 ToVec3(this in Vector2Int v) => new(v.x, v.y, 0);
        public static Vector3 ToVec3XZ(this in Vector2Int v) => new(v.x, 0, v.y);
        public static Vector3 ToVec3XZ(this in Vector2 v) => new(v.x, 0, v.y);
        
        public static Vector3 ToVec3(this in Vector2 v) => new(v.x, v.y, 0f);

        public static Vector2 ToVec2(this in Vector3 v) => new(v.x, v.y);
        
        public static float ToAngle(this Vector2 dir) => Vector2.SignedAngle(DirectionExtension.RefVec, dir);
        public static Vector2 ToVector2(this float angle) => new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        
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
        public static int Random(this in Vector2Int v) => UnityEngine.Random.Range(v.x, v.y);

        public static Vector3 SetX(this Vector3 vec, float x) { vec.x = x; return vec; }
        public static Vector3 SetY(this Vector3 vec, float y) { vec.y = y; return vec; }
        public static Vector3 SetZ(this Vector3 vec, float z) { vec.z = z; return vec; }

        public static float Lerp(this in Vector2 v, float normal)
        {
            return Mathf.Lerp(v.x, v.y, normal);
        }
    }
}