using System;
using Accelib.Data;
using Accelib.Tween.Effect;
using DG.Tweening;
using UnityEngine;

namespace Accelib.Effect
{
    [RequireComponent(typeof(CanvasGroup))]
    // [Obsolete("대신 "+nameof(SimpleEffectTween_Fade)+"을 사용하세요.")]
    public class SimpleFadeEffect : MonoBehaviour
    {
        private enum FadeMode { In, Out, None }
        
        private CanvasGroup group = null;
        [SerializeField] private EasePairTweenConfig config;

        [Header("설정")] 
        [SerializeField] private FadeMode startFadeMode = FadeMode.None;
        [SerializeField] private bool ignoreTimeScale = false;
        [SerializeField] private bool disableOnFadeOut = false;

        private Tweener _tween;

        private void OnEnable()
        {
            if (startFadeMode == FadeMode.In)
                FadeIn();
            else if (startFadeMode == FadeMode.Out)
                FadeOut();
        }

        private void OnDisable() => _tween?.Kill();

        public void ToggleFadeEffect(bool fadeIn)
        {
            if (fadeIn) FadeIn(false);
            else FadeOut(false);
        }

        public Tweener FadeIn(bool clearOnStart = true)
        {
            if (!group && !TryGetComponent(out group))
                return _tween;
            
            if (clearOnStart)
            {
                gameObject.SetActive(true);
                group.alpha = 0f;
            }
            
            _tween?.Kill();
            _tween = group.DOFade(1f, config.duration)
                .SetUpdate(ignoreTimeScale)
                .SetEase(config.easeA)
                .SetDelay(config.delayA);

            return _tween;
        }
        
        public Tweener FadeOut(bool clearOnStart = true)
        {
            if (!group && !TryGetComponent(out group))
                return _tween;
            
            if (clearOnStart)
            {
                gameObject.SetActive(true);
                group.alpha = 1f;
            }
            
            _tween?.Kill();
            _tween = group.DOFade(0f, config.durationA)
                .SetUpdate(ignoreTimeScale)
                .SetEase(config.easeB)
                .SetDelay(config.delayB);
                
            if(disableOnFadeOut) 
                _tween.OnComplete(() => gameObject.SetActive(false));

            return _tween;
        }
        
        public void DoFadeIn() => FadeIn();
        public void DoFadeOut() => FadeOut();
    }
}