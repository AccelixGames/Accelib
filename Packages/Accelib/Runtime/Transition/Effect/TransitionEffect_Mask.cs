using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Transition.Effect
{
    public class TransitionEffect_Mask : TransitionEffect
    {
        [SerializeField, Range(0.01f, 5f)] private float canvasGroupDuration = 0.1f;
        
        [Header("Effect")]
        [SerializeField] private float radius = 652f;
        [SerializeField] private RectTransform target;
        [SerializeField] private CanvasGroup loadingGroup;

        [Button]
        public override async UniTask StartTransition()
        {
            canvas.gameObject.SetActive(true);
            target.sizeDelta = Vector2.one * radius;

            if (loadingGroup)
            {
                loadingGroup.gameObject.SetActive(true);
                loadingGroup.alpha = 0f;
            }

            var seq = DOTween.Sequence().SetLink(gameObject)
            .Append(target.DOSizeDelta(Vector2.zero, duration).SetEase(easeStart));
            
            if(loadingGroup)
                seq.Append(loadingGroup.DOFade(1f, canvasGroupDuration));

            await seq;
        }

        [Button]
        public override async UniTask EndTransition()
        {
            var seq = DOTween.Sequence().SetLink(gameObject)
                .Append(target.DOSizeDelta(Vector2.one * radius, duration).SetEase(easeEnd));
            if (loadingGroup)
                seq.Join(loadingGroup.DOFade(0f, canvasGroupDuration));
            seq.OnComplete(() =>
            {
                canvas.gameObject.SetActive(false);
                if (loadingGroup)
                    loadingGroup.gameObject.SetActive(false);
            });

            await seq;
        }
    }
}