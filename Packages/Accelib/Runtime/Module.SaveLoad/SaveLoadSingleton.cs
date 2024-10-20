using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Core;
using Accelib.Logging;
using Accelib.Module.Initialization;
using Accelib.Module.Initialization.Base;
using Accelib.Module.SaveLoad.SaveDataHolder;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Accelib.Module.SaveLoad
{
    public class SaveLoadSingleton : MonoSingleton<SaveLoadSingleton>, IAsyncInitRequired
    {
        [Header("세이브 데이터들")]
        [SerializedDictionary("타입", "오브젝트")]
        [SerializeField] private SaveDataHolderBase[] holders;
        private Dictionary<Type, SaveDataHolderBase> _holderDict = new();

        [Header("옵션")]
        [SerializeField] private bool initOnStart = true;

        public async UniTask<bool> InitAsync()
        {
            var taskPool = new List<UniTask<bool>>();
            
            _holderDict = new Dictionary<Type, SaveDataHolderBase>();
            foreach (var holder in holders)
            {
                if (!_holderDict.TryAdd(holder.GetType(), holder))
                {
                    Deb.LogError($"Failed to add holder: {holder.name}({holder.GetType()})", holder);
                    continue;
                }

                taskPool.Add(holder.ReadAsync());
            }

            var result = await UniTask.WhenAll(taskPool);
            return result.All(isTrue => isTrue);
        }
        
        public T Get<T>() where T : SaveDataHolderBase
        {
            if (_holderDict.TryGetValue(typeof(T), out var holder))
                return holder as T;

            return null;
        }
        
#if UNITY_EDITOR
        private void Reload()
        {
            holders = GetComponentsInChildren<SaveDataHolderBase>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => Initialize();
#endif
    }
}