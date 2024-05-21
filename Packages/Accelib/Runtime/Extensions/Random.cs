using UnityEngine;

namespace Accelib.Extensions
{
    public static class Random
    {
        public static Vector2Int Vec2Int(int minInclusive, int maxInclusive) => new(
            UnityEngine.Random.Range(minInclusive, maxInclusive + 1),
            UnityEngine.Random.Range(minInclusive, maxInclusive + 1));
    }
}