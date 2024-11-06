using UnityEngine;

namespace Accelib.Module.UI.SafeArea.Architecture
{
    [System.Serializable]
    public struct CustomSafeArea
    {
        public float up;
        public float down;
        public float left;
        public float right;

        public CustomSafeArea(float up, float down, float left, float right)
        {
            this.up = up;
            this.down = down;
            this.left = left;
            this.right = right;
        }

        public void Reset()
        {
            up = 0;
            down = 0;
            left = 0;
            right = 0;
        }

        public void Add(CustomSafeArea other)
        {
            up += other.up;
            down += other.down;
            left += other.left;
            right += other.right;
        }

        public void Multiply(float value)
        {
            up *= value;
            down *= value;
            left *= value;
            right *= value;
        }
    }
}