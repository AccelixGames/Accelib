using UnityEngine;

namespace Accelib.Core
{
    [DefaultExecutionOrder(-1000)]
    public class MonoSingletonStatic<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] protected bool dontDestroyOnLoad = true;
        
        // Check to see if we're about to be destroyed
        private static bool _shuttingDown;
        private static readonly object _lock = new();
        private static T _instance;
        
        /// <summary>
        /// Access singleton instance through this property.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_shuttingDown) return null;

                lock (_lock)
                {
                    if (!_instance)
                    {
                        // Search for existing instance.
                        _instance = FindFirstObjectByType<T>(FindObjectsInactive.Exclude);
                        var singleton = _instance as MonoSingletonStatic<T>;
                        if(singleton?.dontDestroyOnLoad ?? false) DontDestroyOnLoad(_instance);
                    }

                    return _instance;
                }
            }
        }

        private void OnApplicationQuit() => _shuttingDown = true;
        private void OnDestroy() => _shuttingDown = true;
        
        protected static void RuntimeInitOnLoad() => _instance = null;
    }
}