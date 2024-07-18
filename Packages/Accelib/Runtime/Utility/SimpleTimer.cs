using UnityEngine;
using UnityEngine.Serialization;

namespace Accelib.Utility
{
    public class SimpleTimer : MonoBehaviour
    {
        [SerializeField] private bool readyOnStart = true;
        [SerializeField, Range(0f, 1000f)] private float endTime = 1f;
        
        [Header("상태")]
        [SerializeField] private float currTime;
        [field: SerializeField] public bool IsReady { get; private set; }

        private void Awake()
        {
            currTime = 0f;
            IsReady = readyOnStart;
        }

        private void Update()
        {
            if(IsReady) return;
            
            currTime += Time.deltaTime;
            if (currTime >= endTime) IsReady = true;
        }

        public void Restart()
        {
            IsReady = false;
            currTime = 0f;
        }
    }
}