using System;
using Accelib.Logging;
using Accelib.Module.Localization.Helper.Formatter;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Accelib.Module.Localization.Helper
{
    /// <summary>
    /// 언어 변경에 대응하는 TMP
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTMP : LocalizedMonoBehaviour
    {
        [field: SerializeField] public TMP_Text TMP { get; private set; }
        
        // 언어 키
        [Header("키")]
        [ValueDropdown("GetAllKey", AppendNextDrawer = true)]
        [SerializeField] private string key;
        [ValueDropdown("GetAllFont", AppendNextDrawer = true)]
        [SerializeField] private int fontId = 0;
        [ValueDropdown("GetAllFontMaterial", AppendNextDrawer = true)]
        [SerializeField] private int fontMaterialId = 0;

        [Header("옵션")] 
        [SerializeField] private bool loadOnEnable = true;
        [SerializeField] private bool useFormatter = false;
        
        private ILocalizedFormatter _formatter;
        
        public override string LocaleKey => key;
        public override int FontIndex => fontId;
        public override bool LoadOnEnable => loadOnEnable;

        // TMP 캐싱 
        private void Awake()
        {
            TMP ??= GetComponent<TMP_Text>();
            _formatter = GetComponent<ILocalizedFormatter>();
        }

        private void OnEnable()
        {
            if (LoadOnEnable) 
                Reload();
        }
        
        [Button("다시 로드", DisplayParameters = false, DrawResult = false)]
        public string Reload()
        {
            // 현지화된 텍스트 가져오기
            var localizedString = LocalizationSingleton.GetLocalizedString(LocaleKey, this);
            
            // 업데이트 이벤트 호출
            OnLocaleUpdated(localizedString);
            
            // 반환
            return TMP.text;
        }

        /// <summary>키 변경</summary>
        public string ChangeKey(string otherKey)
        {
            key = otherKey;
            return Reload();
        }
        
        public override void OnLocaleUpdated(string localizedString)
        {
            // TMP가 NULL 일 경우, 종료
            TMP ??= GetComponent<TMP_Text>();
            if (!TMP) return;

            // 폰트 업데이트
            var fontData = LocalizationSingleton.GetFontData(fontId);
            if (fontData?.FontAsset)
            {
                TMP.font = fontData.FontAsset;
                TMP.fontMaterial = fontData.GetMaterial(fontMaterialId);
            }
            
            // 포맷 적용
            if (useFormatter)
            {
                var args = _formatter?.GetArgs();
                if (args != null)
                    localizedString = string.Format(localizedString, args);
            }

            // 텍스트 변경
            TMP.SetText(localizedString);
            Deb.Log("Update Locale", this);
        }


        private void Reset()
        {
            TMP = GetComponent<TMP_Text>();

#if UNITY_EDITOR
            Deb_SyncFont();
#endif
        }

#if UNITY_EDITOR
        [BoxGroup("# Debug")]
        [Button("TMP에 Preview 적용", DisplayParameters = false, DrawResult = false)]
        private void Deb_UpdateTMP() => TMP.text = TextPreview;

        [BoxGroup("# Debug")]
        [Button("TMP에서 Font 읽기", DisplayParameters = false, DrawResult = false)]
        private void Deb_SyncFont()
        {
            (fontId, fontMaterialId) = GetFontID(TMP.font, TMP.fontSharedMaterial);
        }
#endif
    }
}