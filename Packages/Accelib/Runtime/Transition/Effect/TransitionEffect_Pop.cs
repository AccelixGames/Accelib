using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Transition.Effect
{
    internal  class TransitionEffect_Pop : TransitionEffect
    {
        [Header("Effect")]
        [SerializeField] private Transform target;
        
        [Button]
        public override async UniTask StartTransition()
        {
            canvas.gameObject.SetActive(true);

            target.localScale = Vector3.one * 0.001f;
            target.localRotation = Quaternion.identity;
            
            await DOTween.Sequence()
                .Join(target.DORotate(new Vector3(0, 0, 360f), duration, RotateMode.FastBeyond360).SetEase(easeStart))
                .Join(target.DOScale(1f, duration).SetEase(easeStart));
        }
        
        [Button]
        public override async UniTask EndTransition()
        {
            await DOTween.Sequence()
                .Join(target.DORotate(new Vector3(0, 0, -360f), duration, RotateMode.FastBeyond360).SetEase(easeEnd))
                .Join(target.DOScale(0f, duration).SetEase(easeEnd))
                .OnComplete(() => canvas.gameObject.SetActive(false));
        }
    }
}