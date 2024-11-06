using Accelib.Module.Localization.Architecture;
using Accelib.Module.Localization.Helper.Formatter;
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
        [Header("키")]
        [SerializeField] private string key;
        [SerializeField] private int fontMaterialId;

        [Header("옵션")]
        [SerializeField] private bool loadOnEnable = true;
        [SerializeField] private bool useFormatter = false;
        
        private ILocalizedFormatter _formatter;
        
        public string LocaleKey => key;
        public bool IsEnabled => enabled;

        // TMP
        public TMP_Text TMP { get; private set; }

        // TMP 캐싱 
        private void Awake()
        {
            TMP = GetComponent<TMP_Text>();
            _formatter = GetComponent<ILocalizedFormatter>();
        }

        private void OnEnable()
        {
            if (!loadOnEnable) return;

            Reload();
        }
        
        [Button("다시 로드", EButtonEnableMode.Playmode)]
        public string Reload()
        {
            // 현지화된 텍스트 가져오기
            var localizedString = LocalizationSingleton.GetLocalizedStringStatic(LocaleKey, this);
            var fontAsset = LocalizationSingleton.GetFontAssetStatic();
            
            // 업데이트 이벤트 호출
            OnLocaleUpdated(localizedString, fontAsset);
            
            // 반환
            return TMP.text;
        }

        public void OnLocaleUpdated(string localizedString, LocaleFontData fontAsset)
        {
            // TMP가 NULL 일 경우, 종료
            TMP ??= GetComponent<TMP_Text>();
            if (!TMP) return;

            if (fontAsset?.FontAsset)
            {
                TMP.font = fontAsset.FontAsset;
                if (fontAsset.FontMaterials != null && fontAsset.FontMaterials.Count > fontMaterialId) 
                    TMP.fontMaterial = fontAsset.FontMaterials[fontMaterialId];
            }
            
            // 포맷 적용
            if (useFormatter)
            {
                var args = _formatter?.GetArgs();
                if (args != null)
                    localizedString = string.Format(localizedString, args);
            }

            // 텍스트 변경
            TMP.text = localizedString;
        }

        /// <summary>
        /// 키 변경
        /// </summary>
        public string ChangeKey(string otherKey)
        {
            key = otherKey;
            return Reload();
        }
    }
}