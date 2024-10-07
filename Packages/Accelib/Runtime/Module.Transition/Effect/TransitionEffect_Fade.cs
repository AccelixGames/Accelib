using Accelib.Module.Audio.Data;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Accelib.Module.Transition.Effect
{
    /// <summary>
    /// 페이드 이펙트
    /// </summary>
    internal  class TransitionEffect_Fade : TransitionEffect
    {
        [Header("Effect")]
        [FormerlySerializedAs("group")]
        [SerializeField] private CanvasGroup target;
        
        [Header("Loading Group")]
        [SerializeField] private CanvasGroup loadingGroup;
        [SerializeField, Range(0.01f, 5f)] private float canvasGroupDuration = 0.2f;
        
        [Header("Audio")]
        [SerializeField] private AudioRefSO fadeStartSfx;
        [SerializeField] private AudioRefSO fadeEndSfx;
        
        private Sequence _seq;
        
        [Button]
        public override Sequence StartTransition()
        {
            canvas.gameObject.SetActive(true);
            target.alpha = 0;
            
            if (loadingGroup)
            {
                loadingGroup.gameObject.SetActive(true);
                loadingGroup.alpha = 0f;
            }
            
            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject)
                .Append(target.DOFade(1f, duration).SetEase(easeStart))
                .JoinCallback(() => fadeStartSfx?.PlayOneShot());
            
            if (loadingGroup)
                _seq.Append(loadingGroup.DOFade(1f, canvasGroupDuration));

            return _seq;
        }

        [Button]
        public override Sequence EndTransition()
        {
            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject)
                .Append(target.DOFade(0f, duration).SetEase(easeEnd))
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