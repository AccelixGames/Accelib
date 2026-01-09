#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Accelib.EditorTool
{
    public static class AssetDatabaseUtil
    {
        public static List<T> FindAllAssets<T>() where T : Object
        {
            Debug.Log($"t: {typeof(T).FullName}");
            
            return UnityEditor.AssetDatabase
                .FindAssetGUIDs($"t: {typeof(T).FullName}")
                .Select(UnityEditor.AssetDatabase.LoadAssetByGUID<T>)
                .ToList();
        }
    }
}
#endif