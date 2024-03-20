using System;
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
        
        private void OnEnable() => Set(startMode);

        public void Toggle() => Set(startMode == Mode.On ? Mode.Off : Mode.On);

        public void Toggle(bool on) => Set(on ? Mode.On : Mode.Off);
        
        private void Set(Mode mode)
        {
            onObj.SetActive(mode == Mode.On);
            offObj.SetActive(mode == Mode.Off);
        }
    }
}