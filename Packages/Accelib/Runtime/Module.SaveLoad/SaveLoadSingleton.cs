using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Core;
using Accelib.Logging;
using Accelib.Module.Initialization.Base;
using Accelib.Module.SaveLoad.SaveDataHolder;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Accelib.Module.SaveLoad
{
    public class SaveLoadSingleton : MonoSingleton<SaveLoadSingleton>, IAsyncInitRequired
    {
        private Dictionary<Type, SaveDataHolderBase> _holderDict = new();

        public async UniTask<bool> InitAsync()
        {
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
            return result.All(isTrue => isTrue);
        }
        
        public static T Get<T>() where T : SaveDataHolderBase
        {
            if (Instance == null)
            {
                Deb.LogError("Instance is null");
                return null;
            }
            
            if (Instance._holderDict.TryGetValue(typeof(T), out var holder))
                return holder as T;

            Deb.LogError($"{typeof(T)}를 찾을 수 없습니다.", Instance);
            return null;
        }
        
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => Initialize();
#endif
    }
}