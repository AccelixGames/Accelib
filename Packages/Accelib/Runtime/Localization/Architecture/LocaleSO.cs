using System.Collections.Generic;
using Accelib.SerializableDictionary;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace Accelib.Localization.Architecture
{
    [CreateAssetMenu(fileName = "Locale-", menuName = "Meow/LocaleSO")]
    public class LocaleSO : ScriptableObject
    {
        [field: Header("언어")] 
        [field: SerializeField] public SystemLanguage Language { get; private set; }
        [field: SerializeField] public LocaleFontData FontData { get; private set; }
        
        [field: Header("통계")]
        [field: SerializeField, ReadOnly] public int Count { get; private set; }
        
        [field: Header("딕셔너리")]
        [field: SerializeField, HideInInspector] public StringStringDictionary TextDict { get; private set; }

        public void FromDictionary(Dictionary<string, string> dict)
        {
            if(dict is not { Count: > 0 }) return;

            TextDict.CopyFrom(dict);
            Count = dict.Count;
            
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
        }
    }
}