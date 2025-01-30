using System;

namespace Accelib.Extension.Atom
{
    [System.Serializable]
    public struct FloatRange : IEquatable<FloatRange>
    {
        public float min;
        public float value;
        public float max;

        public float Random() => UnityEngine.Random.Range(min, max);
        
        public bool Equals(FloatRange other)
        {
            return min.Equals(other.min) && value.Equals(other.value) && max.Equals(other.max);
        }

        public override bool Equals(object obj)
        {
            return obj is FloatRange other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(min, value, max);
        }
    }
}