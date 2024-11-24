using UnityEngine;

namespace Accelib.Module.UI.Toggle
{
    public abstract class ToggleBtn : MonoBehaviour
    {
        [Header("[코어]")] 
        public ToggleBtnGroup Group { get; private set; }
        public bool IsEnabled { get; private set; }
        [field: SerializeField] public int Id { get; private set; }
        
        internal void Initialize(ToggleBtnGroup g, bool isOn)
        {
            Group = g;
            Toggle(isOn, true);
        }

        internal void Toggle(bool isOn, bool forceInitialize = false)
        {
            // 같으면 무시
            if(!forceInitialize && (isOn == IsEnabled)) return;
            
            IsEnabled = isOn;
            OnToggleStateChanged(isOn);
        }

        public void OnClick()
        {
            Group?.Toggle(this);
        }

        /// <summary>
        /// 토글 상태가 변경되었을 때 호출된다.
        /// </summary>
        protected abstract void OnToggleStateChanged(bool isOn);
    }
}