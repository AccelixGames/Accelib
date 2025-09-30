using UnityEngine;

namespace Accelib.Extensions
{
    public static class Random
    {
        public static Vector2Int Vec2Int(int minInclusive, int maxInclusive) => new(
            UnityEngine.Random.Range(minInclusive, maxInclusive + 1),
            UnityEngine.Random.Range(minInclusive, maxInclusive + 1));

        public static Vector3 InsideBounds(Bounds bounds) => new(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );

        public static Vector3 InsideBounds(BoxCollider collider) => 
            InsideBounds(collider.bounds);
    }
}