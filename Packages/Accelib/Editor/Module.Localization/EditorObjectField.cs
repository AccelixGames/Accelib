using System;
using UnityEditor;
using UnityEngine;

namespace Accelix.Editor
{
    [Serializable]
    public class EditorObjectField<T> where T : ScriptableObject
    {
        public string key;
        public T asset;
        public string assetPath;

        public EditorObjectField(string baseKey)
        {
            key = baseKey;

            assetPath = EditorPrefs.GetString(baseKey, "");
            asset = string.IsNullOrEmpty(assetPath) ? null : AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }
        
        public void SaveOnChanged()
        {
            if (asset == null) return;

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