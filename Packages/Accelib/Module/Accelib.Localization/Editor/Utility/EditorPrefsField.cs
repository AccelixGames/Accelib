#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Accelib.Editor.Module.Localization.Utility
{
    /// <summary>
    /// EditorPrefs 기반 에셋 참조 필드. 프로퍼티 기반 접근 패턴.
    /// </summary>
    [System.Serializable]
    public class EditorPrefsField<T> where T : Object
    {
        private T _asset;
        private string _key;
        private string _assetPath;

        public EditorPrefsField(string baseKey)
        {
            _key = baseKey;
            _assetPath = EditorPrefs.GetString(baseKey);
            if(!string.IsNullOrEmpty(_assetPath))
                _asset = string.IsNullOrEmpty(_assetPath) ? null : AssetDatabase.LoadAssetAtPath<T>(_assetPath);
            else
            {
                var guids = AssetDatabase.FindAssetGUIDs($"t:{typeof(T).Name}");
                foreach (var guid in guids)
                {
                    _asset = AssetDatabase.LoadAssetByGUID<T>(guid);

                    if (_asset != null)
                    {
                        _assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        EditorPrefs.SetString(_key, _assetPath);
                        break;
                    }
                }
            }
        }

        public T Asset
        {
            get => _asset;
            set
            {
                _asset = value;
                var newPath = AssetDatabase.GetAssetPath(_asset);
                if (_assetPath != newPath)
                {
                    _assetPath = newPath;
                    EditorPrefs.SetString(_key, _assetPath);
                }
            }
        }
    }
}
#endif
