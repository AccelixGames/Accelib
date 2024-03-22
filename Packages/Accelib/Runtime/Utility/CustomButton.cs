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
        [Header("상태")] 
        [SerializeField] public bool isEnabled = true;
        
        [Header("설정")]
        [SerializeField] private ButtonTweenConfig config;
        [SerializeField] private bool useClick = true;
        [SerializeField] private bool useDown;
        [SerializeField] private bool useUp;
        
        [Header("이벤트")]
        [SerializeField, ShowIf(nameof(useClick))] public UnityEvent onPointerClick = new();
        [SerializeField, ShowIf(nameof(useDown))] public UnityEvent onPointerDown = new();
        [SerializeField, ShowIf(nameof(useUp))] public UnityEvent onPointerUp = new();
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if(!isEnabled) return;
            
            if(useClick) onPointerClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(!isEnabled) return;
            
            transform
                .DOScale(config.DownAmount, config.DownDuration)
                .SetEase(config.DownEase)
                .SetLink(gameObject);
            
            if(useDown) onPointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(!isEnabled) return;
            
            transform
                .DOScale(1f, config.UpDuration)
                .SetEase(config.UpEase)
                .SetLink(gameObject);
            
            if(useUp) onPointerUp?.Invoke();
        }
    }
}