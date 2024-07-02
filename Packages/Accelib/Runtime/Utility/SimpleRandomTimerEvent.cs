using Accelib.Extensions;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Utility
{
    public class SimpleRandomTimerEvent : MonoBehaviour
    {
        [SerializeField] private bool loop = false;
        [SerializeField, MinMaxSlider(0f, 60f)] private Vector2 timeRange = new(2f, 5f);
     
        [Header("Debug")]
        [SerializeField, ReadOnly] private float timer;
        [SerializeField, ReadOnly] private float targetTime;

        public UnityEvent onTime;

        private void OnEnable()
        {
            StartTimer();
        }

        private void Update()
        {
            if(timer < 0f ) return;

            timer += Time.deltaTime;
            if (timer >= targetTime)
            {
                onTime?.Invoke();

                if (!loop) timer = -1f;
                else
                {
                    timer -= targetTime;
                    if (timer <= 0f) timer = 0f;
                    targetTime = timeRange.Random();
                }
            }
        }

        [Button]
        public void StartTimer()
        {
            targetTime = timeRange.Random();
            timer = 0f;
        }
    }
}