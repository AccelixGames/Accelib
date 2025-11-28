using UnityEngine.Events;

namespace Accelib.Module.UI.Toggle.Implements
{
    public class DefaultToggleBtn : ToggleBtn
    {
        public UnityEvent onToggleOn;
        public UnityEvent onToggleOff;
        public UnityEvent<bool> onToggleChanged;
        
        protected override void OnToggleStateChanged(bool isOn)
        {
            if(isOn)
                onToggleOn.Invoke();
            else 
                onToggleOff.Invoke();
            
            onToggleChanged.Invoke(isOn);
        }
    }
}