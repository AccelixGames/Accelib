#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Accelib.EditorTool;
using Accelib.Module.Localization.Architecture;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Accelib.Module.Localization.EditorTool
{
    public static class LocaleUtility
    {
        public static SystemLanguage CurrPreviewLanguage
        {
            get => (SystemLanguage)EditorPrefs.GetInt(nameof(LocaleUtility) + nameof(CurrPreviewLanguage), (int)SystemLanguage.English);
            set => EditorPrefs.SetInt(nameof(LocaleUtility) + nameof(CurrPreviewLanguage), (int)value);
        }
        
        private static List<LocaleSO> LocaleAssets => AssetDatabaseUtil.FindAllAssets<LocaleSO>();
        public static List<SystemLanguage> SupportedLanguages => LocaleAssets?.Select(x => x.Language).ToList();
        
        private static LocaleSO _localeAsset;
        public static LocaleSO LocaleAsset
        {
            get
            {
                // 언어
                var previewLang = CurrPreviewLanguage;
                
                // 기존 것 반환
                if (_localeAsset != null && _localeAsset.Language == previewLang) return _localeAsset;
                
                // 찾기
                _localeAsset = LocaleAssets.FirstOrDefault(x=>x.Language == previewLang);

                // 반환
                return _localeAsset;
            }
        }
        
        public static IReadOnlyList<string> GetLocaleKeys() => LocaleAsset?.GetKeys();
        public static IReadOnlyList<LocaleFontData> GetLocaleFonts() => LocaleAsset?.GetFontDataList();
        
        public static IEnumerable FontDropdownList()
        {
            if (!LocaleAsset) return null;

            var fontList = _localeAsset.GetFontDataList();
            var list = new ValueDropdownList<int>();
            for (var i = 0; i < fontList.Count; i++) 
                list.Add(fontList[i].FontAsset.name, i);
            return list;
        }
        public static IEnumerable FontMaterialDropdownList(int fontIndex)
        {
            if (!LocaleAsset) return null;
            var fontList = _localeAsset.GetFontDataList();
            
            if (fontList.Count <= fontIndex) return null;
            
            var list = new ValueDropdownList<int>();
            var matList = fontList[fontIndex].FontMaterials;
            for (var i = 0; i < matList.Count; i++) 
                list.Add(matList[i].name, i);
            return list;
        }
    }
}
#endif