using System.Collections.Generic;
using System.Linq;
using Accelib.Extensions;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Module.Localization.Architecture
{
    /// <summary>
    /// 로케일 ScriptableObject. 특정 언어의 텍스트 딕셔너리와 폰트 데이터를 관리한다.
    /// </summary>
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

        /// <summary>키에 해당하는 로컬라이즈된 문자열을 가져온다.</summary>
        public bool TryGetValue(string key, out string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                value = string.Empty;
                return false;
            }

            textDict ??= new SerializedDictionary<string, string>();
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
                    Debug.LogError($"Duplicate({key}): {value}");

            count = dict.Count;

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
        }
#endif
    }
}
