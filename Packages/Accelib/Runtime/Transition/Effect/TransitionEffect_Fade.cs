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
        [SerializeField] private CanvasGroup group;
        [SerializeField] private float durationOut;
        
        [Button]
        public override async UniTask StartTransition()
        {
            canvas.gameObject.SetActive(true);

            group.alpha = 0f;
            await group.DOFade(1f, duration)
                .SetEase(easeStart);
        }

        [Button]
        public override async UniTask EndTransition()
        {
            await group.DOFade(0f, durationOut)
                .SetEase(easeEnd)
                .OnComplete(() => canvas.gameObject.SetActive(false));
        }
    }
}