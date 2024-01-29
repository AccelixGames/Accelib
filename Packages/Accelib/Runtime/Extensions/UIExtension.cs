using UnityEngine.UI;

namespace Accelib.Extensions
{
    public static class UIExtension
    {
        public static Graphic SetAlpha(this Graphic target, float alpha)
        {
            var color = target.color;
            color.a = alpha;
            target.color = color;

            return target;
        }
    }
}