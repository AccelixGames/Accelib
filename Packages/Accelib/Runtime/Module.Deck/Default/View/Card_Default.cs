using Accelib.Data;
using Accelib.Module.Deck.Base.Model;
using Accelib.Module.Deck.Base.View;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Accelib.Module.Deck.Default.View
{
    public class Card_Default : CardBase<SO_DeckDataBase>, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Tween Config")]
        [SerializeField] DefaultTweenConfig moveTweenConfig;
        [SerializeField] EasePairTweenConfig scaleTweenConfig;
        [SerializeField] ColorTweenConfig colorTweenConfig;
        
        [Header("Value")]
        [SerializeField] private float moveOffsetY = 0.5f;
        [SerializeField] private float scaleOffset = 1.2f;
        private Vector3 _originScale;

        private Renderer _renderer;
        
        
        public void Start()
        {
            _originScale = transform.localScale;

            _renderer = GetComponent<Renderer>();
        }

        public override void MoveTo(Vector3 position)
        {
            transform.DOMove(position, moveTweenConfig.duration)
                .SetEase(moveTweenConfig.ease);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            // 색상 변경
            _renderer?.material?.DOColor(colorTweenConfig.enabledColor, colorTweenConfig.duration)
                .SetEase(colorTweenConfig.easeA);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // 색상 초기화
            _renderer?.material?.DOColor(colorTweenConfig.disabledColor, colorTweenConfig.duration)
                .SetEase(colorTweenConfig.easeA);
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            
            // 크기 확대
            transform.DOScale(_originScale * scaleOffset, scaleTweenConfig.duration)
                .SetEase(scaleTweenConfig.easeA);
        }
        
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            
            // 크기 초기화
            transform.DOScale(_originScale, scaleTweenConfig.duration)
                .SetEase(scaleTweenConfig.easeB);
        }
    }
}