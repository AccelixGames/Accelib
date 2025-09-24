using Accelib.Data;
using DG.Tweening;
using UnityEngine;

namespace Accelib.Tween.Effect
{
    [RequireComponent(typeof(RectTransform))]
    public class SimpleScaleEffect : MonoBehaviour
    {
        private enum FadeMode { In, Out, None }
        
        [SerializeField] private EasePairTweenConfig config;

        [Header("스케일")]
        [SerializeField] private float inScale = 1f;
        [SerializeField] private float outScale = 1f;
        
        [Header("설정")] 
        [SerializeField] private FadeMode startMode = FadeMode.None;
        [SerializeField] private bool ignoreTimeScale = false;

        private RectTransform _rt;
        private Tweener _tween;
        
        private Vector3 Scale(float value) => Vector3.one * value;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (startMode == FadeMode.In)
                ScaleIn();
            else if (startMode == FadeMode.Out)
                ScaleOut();
        }

        private void OnDisable() => _tween?.Kill();
        
        public Tweener ScaleIn(bool clearOnStart = true)
        {
            if (clearOnStart)
            {
                gameObject.SetActive(true);
                _rt.localScale = Scale(outScale);
            }
            
            _tween?.Kill();
            _tween = _rt.DOScale(Scale(inScale), config.duration)
                .SetUpdate(ignoreTimeScale)
                .SetEase(config.easeA)
                .SetDelay(config.delayA);

            return _tween;
        }
        
        public Tweener ScaleOut(bool clearOnStart = true)
        {
            if (clearOnStart)
            {
                gameObject.SetActive(true);
                _rt.localScale = Scale(inScale);
            }
            
            _tween?.Kill();
            _tween = _rt.DOScale(Scale(outScale), config.duration)
                .SetUpdate(ignoreTimeScale)
                .SetEase(config.easeB)
                .SetDelay(config.delayB);

            return _tween;
        }
        
        public void DoScaleIn() => ScaleIn();
        public void DoScaleOut() => ScaleOut();
    }
}