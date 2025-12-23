using Accelib.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Core
{
    [DefaultExecutionOrder(-1000)]
    public class MonoSingletonSerialized<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
    {
        [SerializeField] private bool dontDestroyOnLoad = false;
        
        public static T Instance { get; private set; }

        public static bool TryGetInstance(out T instance)
        {
            if (!Instance)
            {
                Deb.LogError($"MonoSingleton<{typeof(T).Name}> 의 instance 가 없습니다.");
                instance = null;
                return false;
            }
            
            instance = Instance;
            return true;
        }
        
        protected virtual void Awake()
        {
            Instance = GetComponent<T>();
            
            if(dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }
        
        protected static void Initialize() => Instance = null;
    }
}