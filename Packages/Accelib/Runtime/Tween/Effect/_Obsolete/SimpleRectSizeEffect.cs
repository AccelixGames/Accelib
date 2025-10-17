using Accelib.Data;
using DG.Tweening;
using UnityEngine;

namespace Accelib.Tween.Effect._Obsolete
{
    [RequireComponent(typeof(RectTransform))]
    public class SimpleRectSizeEffect : MonoBehaviour
    {
        private enum FadeMode { In, Out, None }
        
        [SerializeField] private EasePairTweenConfig config;

        [Header("스케일")]
        [SerializeField] private Vector2 inSize = Vector3.one;
        [SerializeField] private Vector2 outSize = Vector3.one;
        
        [Header("설정")] 
        [SerializeField] private FadeMode startMode = FadeMode.None;
        [SerializeField] private bool ignoreTimeScale = false;

        private RectTransform _rt;
        private Tweener _tween;

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
                _rt.sizeDelta = outSize;
            }
            
            _tween?.Kill();
            _tween = _rt.DOSizeDelta(inSize, config.duration)
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
                _rt.sizeDelta = inSize;
            }
            
            _tween?.Kill();
            _tween = _rt.DOSizeDelta(outSize, config.duration)
                .SetUpdate(ignoreTimeScale)
                .SetEase(config.easeB)
                .SetDelay(config.delayB);

            return _tween;
        }
        
        public void DoScaleIn() => ScaleIn();
        public void DoScaleOut() => ScaleOut();
    }
}