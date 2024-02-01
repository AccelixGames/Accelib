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
        
        [SerializeField] private float disableTime;
        [SerializeField] private Mode endMode = Mode.Disable;
    
        private float _timer;

        public void Initialize(float time, Mode mode = Mode.Disable)
        {
            disableTime = time;
            endMode  = mode;
            _timer = 0;
        }
    
        private void OnEnable()
        {
            _timer = 0;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (!(_timer >= disableTime)) return;
        
            _timer = 0;
            
            if (endMode == Mode.Destroy)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}
