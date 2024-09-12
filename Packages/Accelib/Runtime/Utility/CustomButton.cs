using System;
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
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField, Range(0f, 1f)] private float disabledAlpha = 0.5f;
        
        [Header("설정")]
        [SerializeField] private PointerEventData.InputButton clickBtn = PointerEventData.InputButton.Left; 
        [SerializeField] private ButtonTweenConfig config;
        [SerializeField] private bool useClick = true;
        [SerializeField] private bool useDown;
        [SerializeField] private bool useUp;
        
        [Header("이벤트")]
        [SerializeField] private UnityEvent<bool> onEnableStateChanged = new();
        [SerializeField, ShowIf(nameof(useClick))] public UnityEvent onPointerClick = new();
        [SerializeField, ShowIf(nameof(useDown))] public UnityEvent onPointerDown = new();
        [SerializeField, ShowIf(nameof(useUp))] public UnityEvent onPointerUp = new();

        private Tweener _tween;

        private void OnEnable()
        {
            if(canvasGroup)
                canvasGroup.alpha = isEnabled ? 1f : disabledAlpha;
        }

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if(isEnabled == value) return;
                
                isEnabled = value;
                onEnableStateChanged?.Invoke(isEnabled);
                
                if(canvasGroup)
                    canvasGroup.DOFade(isEnabled ? 1f : disabledAlpha, 0.2f).SetEase(Ease.OutSine).SetLink(gameObject);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(!isEnabled) return;
            if (eventData.button != clickBtn) return;
            
            if(useClick) onPointerClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(!isEnabled) return;
            
            _tween?.Kill();
            _tween = transform
                .DOScale(config.DownAmount, config.DownDuration)
                .SetEase(config.DownEase)
                .SetLink(gameObject);
            
            if(useDown) onPointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(!isEnabled) return;

            _tween?.Kill();
            _tween = transform
                .DOScale(1f, config.UpDuration)
                .SetEase(config.UpEase)
                .SetLink(gameObject);
            
            if(useUp) onPointerUp?.Invoke();
        }
    }
}