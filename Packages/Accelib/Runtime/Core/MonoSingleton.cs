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
            // if (Instance != null)
            // {
            //     Debug.LogWarning("이미 인스턴스가 존재합니다:" + gameObject.name);
            //     
            //     Destroy(gameObject);
            //     return;
            // }

            Instance = GetComponent<T>();
            
            if(dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }
    }
}