using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Transition.Effect
{
    internal class TransitionEffect_Mask : TransitionEffect
    {
        [Header("Effect")]
        [SerializeField] private Transform target;
        [SerializeField] private CanvasGroup loadingGroup;
        
        [Button]
        public override async UniTask StartTransition()
        {
            canvas.gameObject.SetActive(true);
            target.localScale = Vector3.one;
            loadingGroup.gameObject.SetActive(true);
            loadingGroup.alpha = 0f;

            await DOTween.Sequence()
                .Append(target.DOScale(0.001f, duration).SetEase(easeStart))
                .Append(loadingGroup.DOFade(1f, duration));
        }
        
        [Button]
        public override async UniTask EndTransition()
        {
            loadingGroup.gameObject.SetActive(false);
            
            await target
                .DOScale(1f, duration)
                .SetEase(easeEnd)
                .OnComplete(() => canvas.gameObject.SetActive(false));
        }
    }
}