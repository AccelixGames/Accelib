using System;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Utility
{
    public class SimpleEvent : MonoBehaviour
    {
        public UnityEvent onAwake;
        public UnityEvent onStart;
        public UnityEvent onEnable;
        public UnityEvent onDisable;
        
        private void Awake() => onAwake?.Invoke();
        private void Start() => onStart?.Invoke();
        private void OnEnable() => onEnable?.Invoke();
        private void OnDisable() => onDisable?.Invoke();
    }
}