using System;
using Accelib.Data;
using Accelib.Tween.Effect;
using DG.Tweening;
using UnityEngine;

namespace Accelib.Effect
{
    [RequireComponent(typeof(CanvasGroup))]
    [Obsolete("대신 "+nameof(SimpleEffectTween_Fade)+"을 사용하세요.")]
    public class SimpleFadeEffect : MonoBehaviour
    {
        private enum FadeMode { In, Out, None }
        
        private CanvasGroup group = null;
        [SerializeField] private EasePairTweenConfig config;

        [Header("설정")] 
        [SerializeField] private FadeMode startFadeMode = FadeMode.None;

        private DG.Tweening.Tween tween;

        private void OnEnable()
        {
            if (startFadeMode == FadeMode.In)
                FadeIn();
            else if (startFadeMode == FadeMode.Out)
                FadeOut();
        }

        private void OnDisable() => tween?.Kill();

        public void ToggleFadeEffect(bool fadeIn)
        {
            if (fadeIn) FadeIn(false);
            else FadeOut(false);
        }

        public DG.Tweening.Tween FadeIn(bool clearOnStart = true)
        {
            group = GetComponent<CanvasGroup>();
            
            if (clearOnStart)
            {
                gameObject.SetActive(true);
                group.alpha = 0f;
            }
            
            tween?.Kill();
            tween = group.DOFade(1f, config.duration)
                .SetEase(config.easeA)
                .SetDelay(config.delay);

            return tween;
        }

        public DG.Tweening.Tween FadeOut(bool clearOnStart = true)
        {
            group = GetComponent<CanvasGroup>();
            
            if (clearOnStart)
            {
                gameObject.SetActive(true);
                group.alpha = 1f;
            }
            
            tween?.Kill();
            tween = group.DOFade(0f, config.duration)
                .SetEase(config.easeB)
                .SetDelay(config.delay)
                .OnComplete(() => gameObject.SetActive(false));

            return tween;
        }
    }
}