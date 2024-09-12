using Accelib.Tween.Effect.Base;
using DG.Tweening;
using UnityEngine;

namespace Accelib.Tween.Effect
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SimpleEffectTween_Fade : SimpleEffectTween
    {
        private CanvasGroup group = null;
        
        protected override Tweener Internal_EnableEffect(bool resetOnStart = true)
        {
            group ??= GetComponent<CanvasGroup>();

            var t = group.DOFade(1f, Config.duration);
            if (resetOnStart)
            {
                gameObject.SetActive(true);
                group.alpha = 0f;
            }

            return t;
        }

        protected override Tweener Internal_DisableEffect(bool resetOnStart = true)
        {
            group ??= GetComponent<CanvasGroup>();

            var t = group.DOFade(0f, Config.duration);
            if (resetOnStart)
            {
                t.OnStart(() =>
                {
                    gameObject.SetActive(true);
                    group.alpha = 1f;
                });
            }
            
            return t;
        }
    }
}