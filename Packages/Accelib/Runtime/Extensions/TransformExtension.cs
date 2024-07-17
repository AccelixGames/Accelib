using UnityEngine;

namespace Accelib.Extensions
{
    public static class TransformExtension
    {
        public static Transform SetX(this Transform tr, float value, bool isLocal = false)
        {
            if (isLocal)
            {
                var pos = tr.localPosition;
                pos.x = value;
                tr.localPosition = pos;   
            }
            else
            {
                var pos = tr.position;
                pos.x = value;
                tr.position = pos;   
            }

            return tr;
        }
        
        public static Transform SetY(this Transform tr, float value, bool isLocal = false)
        {
            if (isLocal)
            {
                var pos = tr.localPosition;
                pos.y = value;
                tr.localPosition = pos;   
            }
            else
            {
                var pos = tr.position;
                pos.y = value;
                tr.position = pos;   
            }

            return tr;
        }
        
        public static Transform SetZ(this Transform tr, float value, bool isLocal = false)
        {
            if (isLocal)
            {
                var pos = tr.localPosition;
                pos.z = value;
                tr.localPosition = pos;   
            }
            else
            {
                var pos = tr.position;
                pos.z = value;
                tr.position = pos;   
            }

            return tr;
        }
    }
}