using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Core
{
    [DefaultExecutionOrder(-1000)]
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [TitleGroup("MonoSingleton", indent: true)]
        [ShowInInspector, ReadOnly, PropertyOrder(-1)] public static T Instance { get; private set; }
        [TitleGroup("MonoSingleton")]
        [SerializeField] private bool dontDestroyOnLoad = false;
        

        public static T GetInstanceSafe()
        {
            if (Instance) return Instance;
            Instance = FindAnyObjectByType<T>();
            return Instance;
        }
        
        public static bool TryGetInstance(out T instance)
        {
            if (!Instance)
            {
                Debug.LogError($"MonoSingleton<{typeof(T).Name}> 의 instance 가 없습니다.");
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