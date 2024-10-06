using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.Transition.Effect
{
    internal  class TransitionEffect_Pop : TransitionEffect
    {
        [Header("Effect")]
        [SerializeField] private Transform target;
        
        [Button]
        public override Sequence StartTransition()
        {
            canvas.gameObject.SetActive(true);

            target.localScale = Vector3.one * 0.001f;
            target.localRotation = Quaternion.identity;
            
            return DOTween.Sequence()
                .Join(target.DORotate(new Vector3(0, 0, 360f), duration, RotateMode.FastBeyond360).SetEase(easeStart))
                .Join(target.DOScale(1f, duration).SetEase(easeStart));
        }
        
        [Button]
        public override Sequence EndTransition()
        {
            return DOTween.Sequence()
                .Join(target.DORotate(new Vector3(0, 0, -360f), duration, RotateMode.FastBeyond360).SetEase(easeEnd))
                .Join(target.DOScale(0f, duration).SetEase(easeEnd))
                .OnComplete(() => canvas.gameObject.SetActive(false));
        }
    }
}