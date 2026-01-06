using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Accelib.Logging;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Accelib.Module.AccelNovel.Control.Resource.Internal
{
    internal interface IResourceHandle
    {
        public UniTask Load(IEnumerable<string> keys);
        public void ReleaseAll();
    }
    
    [Serializable]
    internal class ResourceHandle<T> : IResourceHandle where T : UnityEngine.Object
    {
        [Header("Option")]
        [SerializeField] private bool isLocked = false;
        [SerializeField] private SerializedDictionary<string, T> dict;
        private Dictionary<string, AsyncOperationHandle<T>> _handles;

        [Header("Debug")]
        [SerializeField] private bool showLog = false;
        
        public IReadOnlyDictionary<string, T> Dict => dict;

        private CancellationTokenSource _cts;
        
        public async UniTask Load(IEnumerable<string> keys)
        {
            // 잠겨있으면 종료
            if(isLocked) return;
            
            // Key가 없으면 종료
            if(keys == null) return;
            if(!keys.Any()) return;
            
            // 잠금
            isLocked = true;
            
            // 딕셔너리 생성
            _cts = new CancellationTokenSource();
            dict ??= new SerializedDictionary<string, T>();
            _handles ??= new Dictionary<string, AsyncOperationHandle<T>>();
            
            // 작업
            var tasks = new List<UniTask>();

            // 키를 순회하며,
            foreach (var key in keys)
            {
                if (string.IsNullOrEmpty(key))
                {
                    if(showLog) Deb.LogWarning($"빈 키: {key}");
                    continue;
                }
                
                // 이미 포함된 키라면, 넘어가기
                if (dict.ContainsKey(key))
                {
                    if(showLog) Deb.LogWarning($"중복된 키: {key}", dict[key]);
                    continue;
                }

                // 로드
                tasks.Add(Load(key));
            }

            // 모든 태스크 대기
            await UniTask.WhenAll(tasks);
            
            // 잠금 해제
            isLocked = false;
        }

        private async UniTask Load(string key)
        {
            try
            {
                // 로드
                var handle = Addressables.LoadAssetAsync<T>(key);
                //var hand = Addressables.LoadAssetAsync<T>(key);
                //var task = handle.ToUniTask(cancellationToken: _cts.Token, autoReleaseWhenCanceled: true);
                //dict[key] = await task;
                _handles[key] = handle;
                dict[key] = await handle.ToUniTask(cancellationToken:_cts.Token);
            }
            catch (Exception e)
            {
                if(showLog) Deb.LogWarning($"로드 실패: {key}\n{e}");
                throw;
            }
        }
        
        public void ReleaseAll()
        {
            isLocked = false;
            
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            
            // if (dict is not { Count: > 0 }) return;
            //
            // foreach (var (_, value) in dict) 
            //     Addressables.Release(value);

            if (_handles is { Count: > 0 })
            {
                foreach (var (_,h) in _handles)
                    if(h.IsValid()) Addressables.Release(h);
                
                _handles.Clear();
                Deb.Log($"{typeof(T)} Resource Handle Clear");
            }
            
            dict?.Clear();
        }
    }
}