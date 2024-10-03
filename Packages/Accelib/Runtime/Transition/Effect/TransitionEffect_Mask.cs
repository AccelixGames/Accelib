using Accelib.Audio.Data;
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

        [Header("Audio")]
        [SerializeField] private AudioRefSO fadeStartSfx;
        [SerializeField] private AudioRefSO fadeEndSfx;

        private Sequence _seq;
        
        [Button]
        public override Sequence StartTransition()
        {
            canvas.gameObject.SetActive(true);
            target.sizeDelta = Vector2.one * radius;

            if (loadingGroup)
            {
                loadingGroup.gameObject.SetActive(true);
                loadingGroup.alpha = 0f;
            }

            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject)
                .Append(target.DOSizeDelta(Vector2.zero, duration).SetEase(easeStart))
                .JoinCallback(() => fadeStartSfx?.PlayOneShot());
            
            if(loadingGroup)
                _seq.Append(loadingGroup.DOFade(1f, canvasGroupDuration));

            return _seq;
        }

        [Button]
        public override Sequence EndTransition()
        {
            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject)
                .Append(target.DOSizeDelta(Vector2.one * radius, duration).SetEase(easeEnd))
                .JoinCallback(() => fadeEndSfx?.PlayOneShot());
                
            if (loadingGroup)
                _seq.Join(loadingGroup.DOFade(0f, canvasGroupDuration));
            _seq.OnComplete(() =>
            {
                canvas.gameObject.SetActive(false);
                if (loadingGroup)
                    loadingGroup.gameObject.SetActive(false);
            });

            return _seq;
        }
    }
}