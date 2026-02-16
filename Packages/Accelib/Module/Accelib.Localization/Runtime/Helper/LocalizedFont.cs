using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Accelib.Module.Localization.Helper
{
    /// <summary>
    /// 언어 변경 시 TMP 폰트만 업데이트하는 컴포넌트.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    [DisallowMultipleComponent]
    public class LocalizedFont : LocalizedMonoBehaviour
    {
        [SerializeField] private TMP_Text tmp;

        [Header("키")]
        [ValueDropdown("GetAllFont", AppendNextDrawer = true)]
        [SuffixLabel("@Internal_GetCurrFontName()", Overlay = true)]
        [SerializeField] private int fontId = 0;
        [ValueDropdown("GetAllFontMaterial", AppendNextDrawer = true)]
        [SuffixLabel("@Internal_GetCurrFontMaterialName(fontMaterialId)", Overlay = true)]
        [SerializeField] private int fontMaterialId = 0;

        [Header("옵션")]
        [SerializeField] private bool loadOnEnable = true;

        public override string LocaleKey => string.Empty;
        public override int FontIndex => fontId;
        public override bool LoadOnEnable => loadOnEnable;

        private void OnEnable()
        {
            if (loadOnEnable)
                UpdateFont();
        }

        public override void OnLocaleUpdated(string localizedString)
        {
            tmp ??= GetComponent<TMP_Text>();
            if (!tmp) return;

            UpdateFont();
        }
        private void UpdateFont()
        {
            var fontData = LocalizationSingleton.GetFontData(fontId);
            if (fontData?.FontAsset)
            {
                tmp.font = fontData.FontAsset;
                tmp.fontMaterial = fontData.GetMaterial(fontMaterialId);
            }
        }

        private void Reset()
        {
            tmp ??= GetComponent<TMP_Text>();

#if UNITY_EDITOR
            Deb_SyncFont();
#endif
        }

#if UNITY_EDITOR
        [BoxGroup("# Debug")]
        [Button("TMP에서 Font 읽기", DisplayParameters = false, DrawResult = false)]
        private void Deb_SyncFont()
        {
            (fontId, fontMaterialId) = GetFontID(tmp.font, tmp.fontSharedMaterial);
        }
#endif
    }
}
