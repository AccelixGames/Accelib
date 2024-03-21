using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.Transition.Effect
{
    /// <summary>
    /// 페이드 이펙트
    /// </summary>
    internal  class TransitionEffect_Fade : TransitionEffect
    {
        [Header("Effect")]
        [SerializeField] private Image image;
        
        [Button]
        public override async UniTask StartTransition()
        {
            canvas.gameObject.SetActive(true);
            
            var color = image.color;
            color.a = 0f;
            image.color = color;

            await image.DOFade(1f, duration)
                .SetEase(easeStart);
        }

        [Button]
        public override async UniTask EndTransition()
        {
            await image.DOFade(0f, duration)
                .SetEase(easeEnd)
                .OnComplete(() => canvas.gameObject.SetActive(false));
        }
    }
}