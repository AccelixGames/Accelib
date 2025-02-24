using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Utility
{
    public class SimpleEvent : MonoBehaviour
    {
        public UnityEvent onAwake;
        public UnityEvent onStart;
        public UnityEvent onEnable;
        
        private void Awake() => onAwake?.Invoke();
        private void Start() => onStart?.Invoke();
        private void OnEnable() => onEnable?.Invoke();
    }
}