#if ACCELIB_AIT
namespace Accelib.AccelixWeb.Model
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
    }
}
#endif