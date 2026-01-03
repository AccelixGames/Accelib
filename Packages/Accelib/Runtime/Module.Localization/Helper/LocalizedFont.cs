using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Accelib.Module.Localization.Helper
{
    public class LocalizedFont : LocalizedMonoBehaviour
    {
        [SerializeField] private TMP_Text tmp;
        
        [Header("키")]
        [ValueDropdown("GetAllFont", AppendNextDrawer = true)]
        [SerializeField] private int fontId = 0;
        [ValueDropdown("GetAllFontMaterial", AppendNextDrawer = true)]
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
        
        private void Reset() => tmp ??= GetComponent<TMP_Text>();
    }
}