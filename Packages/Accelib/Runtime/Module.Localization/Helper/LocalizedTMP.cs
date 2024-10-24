﻿using Accelib.Module.Localization.Architecture;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace Accelib.Module.Localization.Helper
{
    /// <summary>
    /// 언어 변경에 대응하는 TMP
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTMP : MonoBehaviour, ILocaleChangedEventListener
    {
        // 언어 키
        [SerializeField] private string key;
        [SerializeField] private int fontMaterialId;

        [Header("Option")]
        [SerializeField] private bool loadOnEnable = true;
        
        public string LocaleKey => key;

        public bool IsEnabled => enabled;

        // TMP
        private TMP_Text _tmp;
        public TMP_Text TMP => _tmp;

        // TMP 캐싱 
        private void Awake() => _tmp = GetComponent<TMP_Text>();

        private void OnEnable()
        {
            if (!loadOnEnable) return;

            Reload();
        }
        
        [Button("다시 로드", EButtonEnableMode.Playmode)]
        public void Reload()
        {
            // 현지화된 텍스트 가져오기
            var localizedString = LocalizationSingleton.GetLocalizedStringStatic(LocaleKey, this);
            var fontAsset = LocalizationSingleton.GetFontAssetStatic();
            
            // 업데이트 이벤트 호출
            OnLocaleUpdated(localizedString, fontAsset);
        }

        public void OnLocaleUpdated(string localizedString, LocaleFontData fontAsset)
        {
            // TMP가 NULL 일 경우, 종료
            if (_tmp == null) return;

            if (fontAsset?.FontAsset != null)
            {
                _tmp.font = fontAsset.FontAsset;
                if (fontAsset.FontMaterials != null && fontAsset.FontMaterials.Count > fontMaterialId) 
                    _tmp.fontMaterial = fontAsset.FontMaterials[fontMaterialId];
            }
            
            // 텍스트 변경
            _tmp.text = localizedString;
        }

        /// <summary>
        /// 키 변경
        /// </summary>
        public void ChangeKey(string otherKey)
        {
            key = otherKey;
            Reload();
        }
    }
}