using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Helper
{
    [System.Serializable]
    public class Timer
    {
        [field: SerializeField] public bool IsEnabled { get;  set; } = false;
        [field: SerializeField] public bool IsDone { get;  set; } = false;
        [field: SerializeField] public float Time { get; set; } = 0f;
        [field: SerializeField] public float TargetTime { get; private set; } = 0f;

        public float TimeLeft => Mathf.Clamp(TargetTime - Time, 0f, TargetTime);

        public float Progress => Mathf.Clamp01(Time / TargetTime);

        public string GetMinSecTime()
        {
            if (Time >= TargetTime) return "00:00";
            
            var t = TargetTime - Time;
            var min = (int)(t / 60f);
            var sec = (int)(t - min * 60f);
            
            return $"{min:00}:{sec:00}";
        }
        
        public bool OnTime(bool skipCount = false)
        {
            if (!IsEnabled) return false;
            
            if(!skipCount && Time < TargetTime) 
                Time += UnityEngine.Time.deltaTime;
            
            if (Time >= TargetTime)
            {
                IsEnabled = false;
                IsDone = true;
                return true;
            }
            
            return false;
        }

        public void Set(float target)
        {
            Time = 0f;
            TargetTime = target;
            IsEnabled = true;
            IsDone = false;
        }

        public void Clear(bool repeat = false)
        {
            if(repeat)
                Time -= TargetTime;
            else
                Time = 0f;
            
            IsEnabled = true;
            IsDone = false;
        }

        public void Stop(bool clear = true)
        {
            if(clear) Time = 0f;
            
            IsEnabled = false;
        }

        public void AddTargetTime(float value) => TargetTime += value;
    }
}