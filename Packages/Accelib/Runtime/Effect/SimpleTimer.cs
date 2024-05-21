using System;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Effect
{
    public class SimpleTimer : MonoBehaviour
    {
        [SerializeField] private float time = 1f;
        [SerializeField] private bool loop = false;
        
        [Header("Event")]
        [SerializeField] private UnityEvent onTime;
        
        private float _timer;
        
        private void OnEnable()
        {
            _timer = 0f;
        }

        private void Update()
        {
            if(_timer < 0 ) return;

            _timer += Time.deltaTime;
            
            if (_timer >= time)
            {
                if (loop) _timer -= time;
                else _timer = -1f;
                
                onTime.Invoke();
            }
        }
    }
}