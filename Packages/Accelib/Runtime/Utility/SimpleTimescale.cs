using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Utility
{
    public class SimpleTimescale : MonoBehaviour
    {
        [SerializeField] private bool setOnAwake = true;
        [SerializeField, Range(0f, 5f)] private float targetTimescale = 1f;
        [ShowInInspector, ReadOnly] private float CurrTimescale => Time.timeScale;
        
        [Button("타임스케일 변경")]
        public void SetTimescale() => Time.timeScale = targetTimescale;

        private void Awake()
        {
            if(setOnAwake) SetTimescale();
        }
    }
}