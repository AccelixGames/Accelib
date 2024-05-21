using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Utility
{
    public class AutoDisable : MonoBehaviour
    {
        public enum Mode
        {
            Disable = 0,
            Destroy = 1
        }
        
        [Header("Option")]
        [SerializeField] private Mode endMode = Mode.Disable;
        [SerializeField] private float disableTime;
    
        [Header("Debug")]
        [SerializeField, ReadOnly] private float timer;

        public void Initialize(float time, Mode mode = Mode.Disable)
        {
            disableTime = time;
            endMode  = mode;
            timer = 0;
        }
    
        private void OnEnable()
        {
            timer = 0;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (!(timer >= disableTime)) return;
        
            timer = 0;
            
            if (endMode == Mode.Destroy)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}
