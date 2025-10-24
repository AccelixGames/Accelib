using Accelib.Data;
using DG.Tweening;
using UnityEngine;

namespace Accelib.Effect
{
    [RequireComponent(typeof(RectTransform))]
    // [Obsolete("대신 "+nameof(SimpleEffectTween_MoveRectTransform)+"을 사용하세요.")]
    public class SimpleMoveEffect : MonoBehaviour
    {
        private enum AutoStart { In = 1, Out = 2, False = 0}
        
        [SerializeField] private RectTransform startPos;
        [SerializeField] private RectTransform endPos;
        [SerializeField] private EasePairTweenConfig config;
        
        [Header("")]
        [SerializeField] private AutoStart autoStartMode = AutoStart.False;

        private RectTransform _rt;
        private Tweener _tween;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (autoStartMode == AutoStart.In)
                EffectIn();
            else if(autoStartMode == AutoStart.Out) EffectOut();
        }

        public void ClearAnchoredPosition()
        {
            _rt ??= GetComponent<RectTransform>();
            _rt.anchoredPosition =  startPos.anchoredPosition;
        }

        public Tweener EffectIn(bool clearOnStart = true)
        {
            if (clearOnStart) 
                _rt.anchoredPosition = startPos.anchoredPosition;
            
            _tween?.Kill();
            return _tween = _rt.DOAnchorPos(endPos.anchoredPosition, config.duration)
                .SetEase(config.easeA, overshoot: config.overshoot)
                .SetDelay(config.delayA);
        }

        public Tweener EffectOut(bool clearOnStart = true)
        {
            if(clearOnStart)
                _rt.anchoredPosition = endPos.anchoredPosition;
            
            _tween?.Kill();
            return _tween = _rt?.DOAnchorPos(startPos.anchoredPosition, config.duration)
                .SetEase(config.easeB , overshoot: config.overshoot)
                .SetDelay(config.delayB);
        }
    }
}