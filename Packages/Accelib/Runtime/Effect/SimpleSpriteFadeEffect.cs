using Accelib.Data;
using Accelib.Extensions;
using DG.Tweening;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimpleSpriteFadeEffect : MonoBehaviour
    {
        private enum FadeMode { In, Out, None }
        
        [SerializeField] private EasePairTweenConfig config;
        [SerializeField] private SpriteRenderer render;
        
        [Header("설정")] 
        [SerializeField, Range(0f, 1f)] private float maxAlpha = 1f;
        [SerializeField] private FadeMode startFadeMode = FadeMode.None;
        private Tweener _tween;
        
        private void OnEnable()
        {
            if (startFadeMode == FadeMode.In)
                FadeIn();
            else if (startFadeMode == FadeMode.Out)
                FadeOut();
        }

        private void OnDisable() => _tween?.Kill();

        public Tweener FadeIn(bool clearOnStart = true)
        {
            if (clearOnStart)
            {
                gameObject.SetActive(true);
                render.SetAlpha(0f);
            }
            
            _tween?.Kill();
            _tween = render.DOFade(maxAlpha, config.duration)
                .SetEase(config.easeA)
                .SetDelay(config.delayA);

            return _tween;
        }
        
        public Tweener FadeOut(bool clearOnStart = true)
        {
            if (clearOnStart)
            {
                gameObject.SetActive(true);
                render.SetAlpha(maxAlpha);
            }
            
            _tween?.Kill();
            _tween = render.DOFade(0f, config.durationA)
                .SetEase(config.easeB)
                .SetDelay(config.delayB)
                .OnComplete(() => gameObject.SetActive(false));

            return _tween;
        }
        
        
        public void ToggleFadeEffect(bool fadeIn)
        {
            if (fadeIn) FadeIn(false);
            else FadeOut(false);
        }
    }
}