using Accelib.Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Accelib.Utility
{
    public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Config")]
        [SerializeField] private ButtonTweenConfig config;
        
        [Header("이벤트")]
        [SerializeField] private UnityEvent onPointerClick = new();
        
        private DG.Tweening.Tween idleTween;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClick.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            idleTween?.Complete();
            
            transform.DOScale(config.DownAmount, config.DownDuration)
                .SetEase(config.DownEase);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            idleTween?.Complete();
            
            transform.DOScale(1f, config.UpDuration)
                .SetEase(config.UpEase);
        }

        private void OnDestroy()
        {
            idleTween?.Kill(true);
            idleTween = null;
        }
    }
}