#if ACCELIB_AIT
using UnityEngine;

namespace Accelib.AccelixWeb.Module.Utility.Model
{
    [System.Serializable]
    public class SafeAreaInsets
    {
        public int top;
        public int bottom;

        public SafeAreaInsets()
        {
            top = 0;
            bottom = 0;
        }
        
        public SafeAreaInsets(int top, int bottom)
        {
            this.top = top;
            this.bottom = bottom;
        }
        
        public SafeAreaInsets(in SafeAreaInsetsRaw raw)
        {
            top = Mathf.RoundToInt(raw.top);
            bottom = Mathf.RoundToInt(raw.bottom);
        }
    }
}
#endif