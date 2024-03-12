using System;
using Accelib.Data;
using DG.Tweening;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimpleFadeEffect : MonoBehaviour
    {
        private enum FadeMode { In, Out, None }
        
        [SerializeField] private CanvasGroup group;
        [SerializeField] private EasePairTweenConfig config;

        [Header("설정")] 
        [SerializeField] private FadeMode startFadeMode = FadeMode.None;

        private DG.Tweening.Tween tween;

        private void OnEnable()
        {
            group.alpha = 0f;

            if (startFadeMode == FadeMode.In)
                FadeIn();
            else if (startFadeMode == FadeMode.Out)
                FadeOut();
        }

        private void OnDisable() => tween?.Kill();

        public DG.Tweening.Tween FadeIn()
        {
            tween?.Kill();
            gameObject.SetActive(true);
            group.alpha = 0f;

            tween = group.DOFade(1f, config.duration)
                .SetEase(config.easeA)
                .SetDelay(config.delay);

            return tween;
        }

        public DG.Tweening.Tween FadeOut()
        {
            tween?.Kill();
            gameObject.SetActive(true);
            group.alpha = 1f;

            tween = group.DOFade(0f, config.duration)
                .SetEase(config.easeB)
                .SetDelay(config.delay)
                .OnComplete(() => gameObject.SetActive(false));

            return tween;
        }
    }
}