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

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (autoStartMode == AutoStart.In)
            {
                _rt.anchoredPosition = startPos.anchoredPosition;
                EffectIn();
            }
            else if(autoStartMode == AutoStart.Out)
            {
                _rt.anchoredPosition = endPos.anchoredPosition;
                EffectOut();
            }
        }

        public void ClearAnchoredPosition()
        {
            _rt ??= GetComponent<RectTransform>();
            _rt.anchoredPosition =  startPos.anchoredPosition;
        }

        public Tweener EffectIn()
        {
            return _rt?.DOAnchorPos(endPos.anchoredPosition, config.duration)
                .SetEase(config.easeA)
                .SetDelay(config.delayA);
        }

        public Tweener EffectOut()
        {
            return _rt?.DOAnchorPos(startPos.anchoredPosition, config.duration)
                .SetEase(config.easeB)
                .SetDelay(config.delayB);
        }
    }
}