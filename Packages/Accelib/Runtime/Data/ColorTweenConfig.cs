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

        public void Do(ref DG.Tweening.Tween tween, Image target, bool enable)
        {
            tween?.Kill();
            
            var c = enable ? enabledColor : disabledColor;
            var ease = enable ? easeA : easeB;

            tween = target.DOColor(c, duration).SetEase(ease);

            //target.color = c;
        }
    }
}