using System.Collections.Generic;
using Accelib.Logging;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.Localization.Architecture
{
    [CreateAssetMenu(fileName = "(Locale) Name", menuName = "Accelib.Utility/LocaleSO")]
    public class LocaleSO : ScriptableObject
    {
        [field: Header("언어")] 
        [field: SerializeField] public SystemLanguage Language { get; private set; }
        [field: SerializeField] public LocaleFontData FontData { get; private set; }
        
        [field: Header("통계")]
        [field: SerializeField, ReadOnly] public int Count { get; private set; }
        
        [field: Header("딕셔너리")]
        [field: SerializeField]
        [field: SerializedDictionary("Key", "Text")]
        public SerializedDictionary<string, string> TextDict { get; private set; }

        #if UNITY_EDITOR
        public void FromDictionary(Dictionary<string, string> dict)
        {
            if(dict is not { Count: > 0 }) return;

            Count = 0;
            
            TextDict = new SerializedDictionary<string, string>(dict);
            TextDict.Clear();
            foreach (var (key, value) in dict) 
                if(!TextDict.TryAdd(key, value))
                    Deb.LogError($"Duplicate({key}): {value}");
            
            Count = dict.Count;
            
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
        }
        #endif
    }
}