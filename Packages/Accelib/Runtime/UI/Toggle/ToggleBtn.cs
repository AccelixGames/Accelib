using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Accelib.UI.Toggle
{
    public class ToggleBtn : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField, ReadOnly] private ToggleBtnGroup group;
        [field: SerializeField] protected bool IsEnabled { get; private set; }

        public void OnPointerClick(PointerEventData eventData) => group.Toggle(this);
        
        internal void Initialized(ToggleBtnGroup g)
        {
            group = g;
        }

        internal void Toggle(bool isOn, bool forceInitialize = false)
        {
            // 같으면 무시
            if(!forceInitialize && (isOn == IsEnabled)) return;
            
            IsEnabled = isOn;
            OnToggleStateChanged(isOn);
        }

        /// <summary>
        /// 토글 상태가 변경되었을 때 호출된다.
        /// </summary>
        protected virtual void OnToggleStateChanged(bool isOn) { }
    }
}