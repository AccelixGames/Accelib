using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Accelib.Utility
{
    public class SimpleTimerEvent : MonoBehaviour
    {
        [SerializeField] private float duration = 1f;
        [SerializeField, ReadOnly] private float timer;

        public UnityEvent onTime;

        private void OnEnable()
        {
            StartTimer();
        }

        private void Update()
        {
            if(timer < 0f ) return;

            timer += Time.deltaTime;
            if (timer >= duration)
            {
                onTime?.Invoke();   
                timer = -1f;
            }
        }

        [Button]
        public void StartTimer() => timer = 0f;
    }
}