using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.Data
{
    [CreateAssetMenu(fileName = "tween-", menuName = "Accelix/Tween/Color", order = 0)]
    public class ColorTweenConfig : EasePairTweenConfig
    {
        public Color enabledColor = Color.black;
        public Color disabledColor = Color.black;

        public Tweener Do(Image target, bool enable)
        {
            var c = enable ? enabledColor : disabledColor;
            var ease = enable ? easeA : easeB;

            return target.DOColor(c, duration).SetEase(ease);
        }
    }
}