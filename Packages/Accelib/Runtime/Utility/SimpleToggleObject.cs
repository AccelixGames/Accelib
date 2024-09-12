using System;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Utility
{
    public class SimpleToggleObject : MonoBehaviour
    {
        private enum Mode {On = 0, Off}
        
        [Header("Target")]
        [SerializeField] private GameObject onObj;
        [SerializeField] private GameObject offObj;

        [Header("StartMode")] 
        [SerializeField] private Mode startMode = Mode.On;
        [SerializeField, ReadOnly] private Mode current = Mode.On;
        
        private void OnEnable() => Set(startMode);

        public void Toggle() => Set(current == Mode.On ? Mode.Off : Mode.On);

        public void Toggle(bool on) => Set(on ? Mode.On : Mode.Off);
        
        private void Set(Mode mode)
        {
            current = mode;
            
            if(onObj != null) onObj.SetActive(mode == Mode.On);
            if(offObj != null) offObj.SetActive(mode == Mode.Off);
        }
    }
}