using System;
using System.Collections.Generic;
using Accelib.Core;
using Accelib.Logging;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using Accelib.Module.AccelNovel.Control.Resource.Internal;

namespace Accelib.Module.AccelNovel.Control.Resource
{
    public class ResourceProvider : MonoSingleton<ResourceProvider>
    {
        // [SerializeField] private 
        
        [Header("Cached Resources")]
        [SerializeField] private ResourceHandle<Sprite> spriteHandle;
        [SerializeField] private ResourceHandle<AudioClip> clipHandle;
        [SerializeField] private ResourceHandle<GameObject> gameObjectHandle;
        
        private Dictionary<Type, IResourceHandle> _handles;

        public Sprite GetSprite(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Deb.LogError("빈 키의 이미지를 로드하려고 했습니다!", this);
                return null;
            }
            return spriteHandle?.Dict?.GetValueOrDefault(key, null);
        }

        public AudioClip GetClip(string key)
        {  
            if (string.IsNullOrEmpty(key))
            {
                Deb.LogError("빈 키의 오디오 클립을 로드하려고 했습니다!", this);
                return null;
            }
            return clipHandle?.Dict?.GetValueOrDefault(key, null);
        }
        
        public async UniTask Load(IEnumerable<string> sprites, IEnumerable<string> clips)
        {
            spriteHandle ??= new ResourceHandle<Sprite>();
            clipHandle ??= new ResourceHandle<AudioClip>();
            
            _handles.TryAdd(typeof(Sprite), spriteHandle);
            _handles.TryAdd(typeof(AudioClip), clipHandle);
            
            await (spriteHandle.Load(sprites), clipHandle.Load(clips));
        }

        public GameObject GetGameObject(string key, bool autoInstantiate = true)
        {
            if (string.IsNullOrEmpty(key))
            {
                Deb.LogError("빈 키의 게임 오브젝트를 로드하려고 했습니다!", this);
                return null;
            }

            if (autoInstantiate)
            {
                var loaded = gameObjectHandle?.Dict?.GetValueOrDefault(key, null);
                
                if (loaded == null)
                {
                    Deb.LogError("오브젝트가 없습니다!", this);
                    return null;
                }


                var instantiated = Instantiate(loaded);
                
                return instantiated;
            }
            else
            {
                return gameObjectHandle?.Dict?.GetValueOrDefault(key, null);
            }
            
        }
        
        
        private void EnsureHandles()
        {
            _handles ??= new Dictionary<Type, IResourceHandle>();

            // SerializeField로 세팅돼있을 수도 있으니 null이면 생성
            spriteHandle ??= new ResourceHandle<Sprite>();
            clipHandle ??= new ResourceHandle<AudioClip>();
            gameObjectHandle ??= new ResourceHandle<GameObject>();

            // 등록(이미 있으면 덮지 않게)
            _handles.TryAdd(typeof(Sprite), spriteHandle);
            _handles.TryAdd(typeof(AudioClip), clipHandle);
            _handles.TryAdd(typeof(GameObject), gameObjectHandle);
        }
        
        public UniTask Load<T>(IEnumerable<string> keys) where T : UnityEngine.Object
        {
            EnsureHandles();

            if (!_handles.TryGetValue(typeof(T), out var h))
            {
                Deb.LogError($"ResourceHandle<{typeof(T).Name}> 이(가) ResourceProvider에 등록되어있지 않습니다.", this);
                return UniTask.CompletedTask;
            }
            
            //if(keys.Count() > )

            return h.Load(keys);
        }
        
        public async UniTask<bool> Load(params ILoadRequest[] requests)
        {
            try
            {
                await UniTask.WhenAll(requests.Select(r => r.Execute(this))).Timeout(TimeSpan.FromSeconds(30));
            }
            catch (TimeoutException)
            {
                Deb.LogWarning("TimeOut!");
                return false;
            }
            catch (Exception e)
            {
                Deb.LogWarning(e.Message);
                return false;
            }
            
            return true;
        }

        [Button]
        [ContextMenu(nameof(ReleaseAll))]
        public void ReleaseAll()
        {
            // spriteHandle?.ReleaseAll();
            // clipHandle?.ReleaseAll();
            // gameObjectHandle?.ReleaseAll();
            
            EnsureHandles();
            
            foreach (var h in _handles)
            {
                h.Value?.ReleaseAll();
            }
        }

        private void OnDestroy() => ReleaseAll();
    }
    
    public interface ILoadRequest
    {
        UniTask Execute(ResourceProvider provider);
    }

    public readonly struct LoadRequest<T> : ILoadRequest where T : UnityEngine.Object
    {
        private readonly IEnumerable<string> _keys;
        public LoadRequest(IEnumerable<string> keys) => _keys = keys;

        public UniTask Execute(ResourceProvider provider) => provider.Load<T>(_keys);
    }

}
