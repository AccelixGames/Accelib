using System;
using Accelib.Data;
using Accelib.Logging;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Accelib.Utility
{
    public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, ISubmitHandler
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

        private Sequence _seq;

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
            if (!isEnabled) return;
            if (eventData.button != clickBtn) return;
            
            if(useClick) onPointerClick?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isEnabled) return;

            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject)
                .Append(DownUpTween(true));

            if (useDown) onPointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(!isEnabled) return;
            
            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject)
                .Append(DownUpTween(false));
            
            if(useUp) onPointerUp?.Invoke();
        }

        private Tweener DownUpTween(bool isDown)
        {
            var amount = isDown ? config.DownAmount : Vector3.one;
            var duration = isDown ? config.DownDuration : config.UpDuration;
            var ease = isDown ? config.DownEase : config.UpEase;
            return transform.DOScale(amount, duration).SetEase(ease);
        }

        public void ShortcutClick()
        {
            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject)
                .Append(DownUpTween(true))
                .Append(DownUpTween(false));
            
            if(useClick) onPointerClick?.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject)
                .Append(DownUpTween(true))
                .Append(DownUpTween(false));
            
            if(useClick) onPointerClick?.Invoke();
        }
    }
}