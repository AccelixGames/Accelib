using System.IO;
using UnityEditor;
using UnityEngine;

namespace Accelib.Editor.Extension
{
    public static class SOUtility
    {
        public static T CreateAsset<T>(string name, string folderPath) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            asset.name = name;
            
            var assetName = Path.GetFileNameWithoutExtension(name) + ".asset";
            var assetPath = Path.Combine(folderPath, assetName);
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssetIfDirty(asset);

            return asset;
        }
    }
}