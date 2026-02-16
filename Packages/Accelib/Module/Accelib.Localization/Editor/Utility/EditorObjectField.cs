#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Accelib.Editor.Module.Localization.Utility
{
    /// <summary>
    /// EditorPrefs 기반 ScriptableObject 참조 필드. 에디터 윈도우에서 에셋 참조를 영속적으로 저장한다.
    /// </summary>
    [Serializable]
    public class EditorObjectField<T> where T : ScriptableObject
    {
        public string key;
        public T asset;
        public string assetPath;

        public EditorObjectField(string baseKey, bool autoLoad = false)
        {
            key = baseKey;

            assetPath = EditorPrefs.GetString(baseKey, "");
            if(!string.IsNullOrEmpty(assetPath))
                asset = string.IsNullOrEmpty(assetPath) ? null : AssetDatabase.LoadAssetAtPath<T>(assetPath);
            else if (autoLoad)
            {
                var guids = AssetDatabase.FindAssetGUIDs($"t:{typeof(T).Name}");
                foreach (var guid in guids)
                {
                    asset = AssetDatabase.LoadAssetByGUID<T>(guid);

                    if (asset != null)
                    {
                        assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        EditorPrefs.SetString(key, assetPath);
                        break;
                    }
                }
            }
        }

        public void SaveOnChanged()
        {
            if (!asset) return;

            var newPath = AssetDatabase.GetAssetPath(asset);
            if (assetPath != newPath)
            {
                assetPath = newPath;
                EditorPrefs.SetString(key, assetPath);
            }
        }

        public void GUILayout(string name) => asset = EditorGUILayout.ObjectField(name, asset, typeof(T), false) as T;
    }
}
#endif
