using Accelib.Data;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Accelib.Utility
{
    public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Config")]
        [SerializeField] private ButtonTweenConfig config;
        [SerializeField] private bool useClick = true;
        [SerializeField] private bool useDown;
        [SerializeField] private bool useUp;
        
        [Header("이벤트")]
        [SerializeField, ShowIf(nameof(useClick))] private UnityEvent onPointerClick = new();
        [SerializeField, ShowIf(nameof(useDown))] private UnityEvent onPointerDown = new();
        [SerializeField, ShowIf(nameof(useUp))] private UnityEvent onPointerUp = new();
        
        private DG.Tweening.Tween idleTween;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if(useClick)
                onPointerClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            idleTween?.Complete();
            
            transform.DOScale(config.DownAmount, config.DownDuration)
                .SetEase(config.DownEase);
            
            if(useDown)
                onPointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            idleTween?.Complete();
            
            transform.DOScale(1f, config.UpDuration)
                .SetEase(config.UpEase);
            
            if(useUp)
                onPointerUp?.Invoke();
        }

        private void OnDestroy()
        {
            idleTween?.Kill(true);
            idleTween = null;
        }
    }
}