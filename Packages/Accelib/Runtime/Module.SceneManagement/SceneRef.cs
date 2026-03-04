using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Accelib.Module.SceneManagement
{
    /// <summary>
    /// Addressable 또는 Built-in 씬을 통합하여 참조하는 직렬화 가능한 구조체.
    /// </summary>
    [Serializable]
    public struct SceneRef
    {
        [SerializeField] private bool _isBuiltIn;

        [ShowIf("_isBuiltIn")]
        [NaughtyAttributes.Scene]
        [SerializeField] private string _builtInSceneName;

        [HideIf("_isBuiltIn")]
        [SerializeField] private AssetReference _addressableRef;

        public bool IsBuiltIn => _isBuiltIn;
        public string BuiltInSceneName => _builtInSceneName;
        public AssetReference AddressableRef => _addressableRef;

        /// <summary>이 참조가 유효한지 확인한다.</summary>
        public bool IsValid()
        {
            if (_isBuiltIn)
                return !string.IsNullOrEmpty(_builtInSceneName);

            return _addressableRef != null && _addressableRef.RuntimeKeyIsValid();
        }

        /// <summary>Addressable 씬으로 생성한다.</summary>
        public SceneRef(AssetReference assetRef)
        {
            _isBuiltIn = false;
            _builtInSceneName = null;
            _addressableRef = assetRef;
        }

        /// <summary>Built-in 씬으로 생성한다.</summary>
        public SceneRef(string sceneName)
        {
            _isBuiltIn = true;
            _builtInSceneName = sceneName;
            _addressableRef = null;
        }

        /// <summary>AssetReference에서 SceneRef로 암묵적 변환 (하위 호환).</summary>
        public static implicit operator SceneRef(AssetReference assetRef) =>
            new SceneRef(assetRef);
    }
}
