using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Accelib.Module.SceneManagement
{
    /// <summary>씬 참조 유형.</summary>
    public enum ESceneRefType
    {
        Addressable = 0,
        BuiltIn = 1,
    }

    /// <summary>
    /// Addressable 또는 Built-in 씬을 통합하여 참조하는 직렬화 가능한 구조체.
    /// </summary>
    [Serializable]
    public struct SceneRef
    {
        [FormerlySerializedAs("_isBuiltIn")]
        [SerializeField] private ESceneRefType _type;

        [SerializeField] private string _builtInSceneName;

        [SerializeField] private AssetReference _addressableRef;

        public ESceneRefType Type => _type;
        public bool IsBuiltIn => _type == ESceneRefType.BuiltIn;
        public string BuiltInSceneName => _builtInSceneName;
        public AssetReference AddressableRef => _addressableRef;

        /// <summary>이 참조가 유효한지 확인한다.</summary>
        public bool IsValid()
        {
            if (_type == ESceneRefType.BuiltIn)
                return !string.IsNullOrEmpty(_builtInSceneName);

            return _addressableRef != null && _addressableRef.RuntimeKeyIsValid();
        }

        /// <summary>Addressable 씬으로 생성한다.</summary>
        public SceneRef(AssetReference assetRef)
        {
            _type = ESceneRefType.Addressable;
            _builtInSceneName = null;
            _addressableRef = assetRef;
        }

        /// <summary>Built-in 씬으로 생성한다.</summary>
        public SceneRef(string sceneName)
        {
            _type = ESceneRefType.BuiltIn;
            _builtInSceneName = sceneName;
            _addressableRef = null;
        }

        /// <summary>AssetReference에서 SceneRef로 암묵적 변환 (하위 호환).</summary>
        public static implicit operator SceneRef(AssetReference assetRef) =>
            new SceneRef(assetRef);
    }
}
