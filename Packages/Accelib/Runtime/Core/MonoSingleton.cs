﻿using Accelib.Logging;
using UnityEngine;

namespace Accelib.Core
{
    [DefaultExecutionOrder(-1000)]
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private bool dontDestroyOnLoad = false;
        
        public static T Instance { get; protected set; }

        protected static bool TryGetInstance(out T instance)
        {
            if (Instance == null)
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