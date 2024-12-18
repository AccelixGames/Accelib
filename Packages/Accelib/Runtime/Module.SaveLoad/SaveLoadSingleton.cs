using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Core;
using Accelib.Logging;
using Accelib.Module.Initialization.Base;
using Accelib.Module.SaveLoad.Config;
using Accelib.Module.SaveLoad.RemoteStorage;
using Accelib.Module.SaveLoad.RemoteStorage.Base;
using Accelib.Module.SaveLoad.SaveDataHolder;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Accelib.Module.SaveLoad
{
    public class SaveLoadSingleton : MonoSingleton<SaveLoadSingleton>, IInitRequired
    {
        [Header("Storage")]
        [SerializeField, ReadOnly] private string remoteStorageName;
        [SerializeField] private bool isInitialized = false;
        
        public static IRemoteStorage RemoteStorage;
        public static SaveLoadConfig Config;
        
        private Dictionary<Type, SaveDataHolderBase> _holderDict = new();
        
        public void Init() => InitAsync().Forget();

        public bool IsInitialized() => isInitialized;

        public async UniTaskVoid InitAsync()
        {
            Config = SaveLoadConfig.Load();
            RemoteStorage = RemoteStorageSelector.GetRemoteStorage(Config.ForceLocalStorage);
            remoteStorageName = RemoteStorage.Name;
            // Deb.Log("RemoteStorage Initialized: " + remoteStorageName);
            
            // 초기화
            var taskPool = new List<UniTask<bool>>();
            
            _holderDict = new Dictionary<Type, SaveDataHolderBase>();
            foreach (var holder in GetComponentsInChildren<SaveDataHolderBase>())
            {
                if (!_holderDict.TryAdd(holder.GetType(), holder))
                {
                    Deb.LogError($"Failed to add holder: {holder.name}({holder.GetType()})", holder);
                    continue;
                }

                taskPool.Add(holder.ReadAsync());
            }

            var result = await UniTask.WhenAll(taskPool);

            isInitialized = true;
        }
        
        public static T Get<T>(Object ctx = null) where T : SaveDataHolderBase
        {
            if (Instance == null)
            {
                Deb.LogError("Instance is null", ctx);
                return null;
            }
            
            if (Instance._holderDict.TryGetValue(typeof(T), out var holder))
                return holder as T;

            Deb.LogError($"{typeof(T)}를 찾을 수 없습니다.", Instance);
            return null;
        }
      
#if UNITY_SWITCH && !UNITY_EDITOR
        public static void WritePlayerPrefs() => ((RemoteStorage_Switch)RemoteStorage).WritePlayerPrefs().Forget();
        public static void ReadPlayerPrefs() => ((RemoteStorage_Switch)RemoteStorage).ReadPlayerPrefs().Forget();
#endif
        
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInit()
        {
            Initialize();

            Config = null;
            RemoteStorage = null;
        }
#endif
    }
}