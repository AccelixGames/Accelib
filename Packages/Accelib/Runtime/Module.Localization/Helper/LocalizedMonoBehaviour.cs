using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Accelib.Extensions;
using Accelib.Module.Localization.Architecture;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Accelib.Module.Localization.Helper
{
    public abstract class LocalizedMonoBehaviour : MonoBehaviour, ILocaleChangedEventListener
    {
        public abstract string LocaleKey { get; }
        public abstract int FontIndex { get; }
        public virtual bool IsEnabled => enabled;
        public abstract bool LoadOnEnable { get; }
        public abstract void OnLocaleUpdated(string localizedString);
                
#if UNITY_EDITOR
        [BoxGroup("# Debug", order:100)]
        [ValueDropdown("GetAllLanguages"), OnValueChanged("FindLocaleAsset")]
        [ShowInInspector] private static SystemLanguage _previewLanguage = SystemLanguage.Korean;
        [BoxGroup("# Debug")]
        [ShowInInspector, ReadOnly] private static LocaleSO _localeAsset;
        [BoxGroup("# Debug")]
        [ShowInInspector, ReadOnly] protected string TextPreview
        {
            get
            {
                if (string.IsNullOrEmpty(LocaleKey) || !_localeAsset) return string.Empty;
                if (!_localeAsset.TryGetValue(LocaleKey, out var value)) return string.Empty;
                
                return value;
            }
        }

        private static List<LocaleSO> _localeAssets;

        protected static bool FindLocaleAsset()
        {
            if (_localeAsset != null && _localeAsset.Language == _previewLanguage) return true;

            _localeAsset = null;
            _localeAssets = new List<LocaleSO>();
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(LocaleSO)}");
            foreach (var guid in guids)
            {
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<LocaleSO>(
                    UnityEditor.AssetDatabase.GUIDToAssetPath(guid));
                _localeAssets.Add(asset);
                if (asset.Language == _previewLanguage) 
                    _localeAsset = asset;
            }

            return _localeAsset;
        }

        protected static IEnumerable<string> GetAllKey()
        {
            if (!FindLocaleAsset()) return null;

            return _localeAsset.GetKeys();
        }
        
        protected static IEnumerable GetAllFont()
        {
            if (!FindLocaleAsset()) return null;

            var fontList = _localeAsset.GetFontDataList();
            var list = new ValueDropdownList<int>();
            for (var i = 0; i < fontList.Count; i++) 
                list.Add(fontList[i].FontAsset.name, i);
            return list;
        }
        
        protected IEnumerable GetAllFontMaterial()
        {
            if (!FindLocaleAsset()) return null;
            var fontList = _localeAsset.GetFontDataList();
            
            if (fontList.Count <= FontIndex) return null;
            
            var list = new ValueDropdownList<int>();
            var matList = fontList[FontIndex].FontMaterials;
            for (var i = 0; i < matList.Count; i++) 
                list.Add(matList[i].name, i);
            return list;
        }

        protected IEnumerable<SystemLanguage> GetAllLanguages() => _localeAssets.Select(x => x.Language);

        protected (int, int) GetFontID(TMP_FontAsset fontAsset, Material fontMat)
        {
            var fontId = 0;
            var fontMatId = 0;
            
            var fontList = _localeAsset.GetFontDataList();
            for (var i = 0; i < fontList.Count; i++)
            {
                if (fontList[i].FontAsset == fontAsset)
                {
                    var mats = fontList[i].FontMaterials;
                    
                    fontId = i;
                    fontMatId = Mathf.Clamp(mats.IndexOf(fontMat), 0, mats.Count - 1);
                    
                    break;
                }
            }
            
            return (fontId, fontMatId);
        }
        
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            _localeAsset = null;
            _localeAssets = null;
        }
#endif
    }
}