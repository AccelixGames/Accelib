using Accelib.Data;
using DG.Tweening;
using UnityEngine;

namespace Accelib.Tween.Effect
{
    [RequireComponent(typeof(RectTransform))]
    public class SimpleMoveEffect_WithPos : MonoBehaviour
    {
        private enum AutoStart { In = 1, Out = 2, False = 0}
        
        [SerializeField] private Vector2 startPos;
        [SerializeField] private Vector2 endPos;
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
                _rt.anchoredPosition = startPos;
                EffectIn();
            }
            else if(autoStartMode == AutoStart.Out)
            {
                _rt.anchoredPosition = endPos;
                EffectOut();
            }
        }

        public void EffectIn()
        {
            _rt?.DOAnchorPos(endPos, config.duration)
                .SetEase(config.easeA)
                .SetDelay(config.delay);
        }

        public void EffectOut()
        {
            _rt?.DOAnchorPos(startPos, config.duration)
                .SetEase(config.easeB)
                .SetDelay(config.delay);
        }
    }
}