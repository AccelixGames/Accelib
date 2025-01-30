using System;

namespace Accelib.Extension.Atom
{
    [System.Serializable]
    public struct IntRange : IEquatable<IntRange>
    {
        public int min;
        public int value;
        public int max;

        public int Random() => UnityEngine.Random.Range(min, max);

        public bool Equals(IntRange other)
        {
            return min == other.min && value == other.value && max == other.max;
        }

        public override bool Equals(object obj)
        {
            return obj is IntRange other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(min, value, max);
        }
    }
}