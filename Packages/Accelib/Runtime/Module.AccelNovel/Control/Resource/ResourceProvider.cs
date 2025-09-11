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
        [Header("Cached Resources")]
        [SerializeField] private ResourceHandle<Sprite> spriteHandle;
        [SerializeField] private ResourceHandle<AudioClip> clipHandle;

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
            
            await (spriteHandle.Load(sprites), clipHandle.Load(clips));
        }

        [Button]
        [ContextMenu(nameof(ReleaseAll))]
        public void ReleaseAll()
        {
            spriteHandle?.ReleaseAll();
            clipHandle?.ReleaseAll();
        }

        private void OnDestroy() => ReleaseAll();
    }
}
