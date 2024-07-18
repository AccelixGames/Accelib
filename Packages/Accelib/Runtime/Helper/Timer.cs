using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Helper
{
    [System.Serializable]
    public class Timer
    {
        [field: SerializeField] public float Time { get; private set; } = 0f;
        [field: SerializeField] public float TargetTime { get; private set; } = 0f;

        public float TimeLeft => Mathf.Clamp(TargetTime - Time, 0f, TargetTime);

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
            if(!skipCount && Time < TargetTime) Time += UnityEngine.Time.deltaTime;
            return Time >= TargetTime;
        }

        public void Set(float target)
        {
            Time = 0f;
            TargetTime = target;
        }

        public void AddTargetTime(float value) => TargetTime += value;
    }
}