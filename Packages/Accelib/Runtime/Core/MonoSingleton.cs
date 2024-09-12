using UnityEngine;

namespace Accelib.Core
{
    [DefaultExecutionOrder(-1000)]
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private bool dontDestroyOnLoad = false;
        
        public static T Instance { get; protected set; }
        
        protected virtual void Awake()
        {
            Instance = GetComponent<T>();
            
            if(dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }
        
        protected static void Initialize() => Instance = null;
    }
}