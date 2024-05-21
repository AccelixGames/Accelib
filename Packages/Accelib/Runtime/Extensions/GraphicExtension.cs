using UnityEngine;
using UnityEngine.UI;

namespace Accelib.Extensions
{
    public static class GraphicExtension
    {
        public static SpriteRenderer SetAlpha(this SpriteRenderer target, float alpha)
        {
            var color = target.color;
            color.a = alpha;
            target.color = color;

            return target;
        }
        
        public static Graphic SetAlpha(this Graphic target, float alpha)
        {
            var color = target.color;
            color.a = alpha;
            target.color = color;

            return target;
        }
    }
}