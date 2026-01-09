using System.Collections.Generic;
using System.Linq;
using Accelib.Extensions;
using Accelib.Logging;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.Module.Localization.Architecture
{
    [CreateAssetMenu(fileName = "(Locale) Name", menuName = "Accelib.Utility/LocaleSO")]
    public class LocaleSO : ScriptableObject
    {
        [Title("# 언어")]
        [SerializeField] private SystemLanguage language;
        [SerializeField] private List<LocaleFontData> fontDataList = new();
        [SerializeField] private LocaleFontData fallbackFont;

        [Title("# 값")] 
        [SerializeField, ReadOnly] private int count;
        [SerializeField, SerializedDictionary("Key", "Text")] private SerializedDictionary<string, string> textDict;

        public SystemLanguage Language => language;
        public LocaleFontData GetFontData(int index) => fontDataList?.GetOrDefault(index, fallbackFont);

        public bool TryGetValue(string key, out string value)
        {
            textDict ??=  new SerializedDictionary<string, string>(textDict);
            return textDict.TryGetValue(key, out value);
        }

#if UNITY_EDITOR
        public IReadOnlyList<string> GetKeys() => textDict?.Keys.ToList();
        public IReadOnlyList<LocaleFontData> GetFontDataList() => fontDataList;
        
        public void FromDictionary(Dictionary<string, string> dict)
        {
            if(dict is not { Count: > 0 }) return;

            count = 0;
            
            textDict ??= new SerializedDictionary<string, string>(dict);
            textDict.Clear();
            foreach (var (key, value) in dict) 
                if(!textDict.TryAdd(key, value))
                    Deb.LogError($"Duplicate({key}): {value}");
            
            count = dict.Count;
            
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
        }
#endif
    }
}