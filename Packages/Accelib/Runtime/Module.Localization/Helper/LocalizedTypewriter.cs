using System;
using Accelib.Module.Localization.Architecture;
using Febucci.UI.Core;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Accelib.Module.Localization.Helper
{
    public class LocalizedTypewriter : LocalizedMonoBehaviour
    {
        [Required(InfoMessageType.Error)]
        [SerializeField] private TypewriterCore typewriter;
        [SerializeField, ReadOnly] private TMP_Text tmp;
        
        [Header("키")]
        [SerializeField, ReadOnly] private string key;
        [ValueDropdown("GetAllFont", AppendNextDrawer = true)]
        [SerializeField] private int fontId = 0;
        [ValueDropdown("GetAllFontMaterial", AppendNextDrawer = true)]
        [SerializeField] private int fontMaterialId = 0;
        
        [Header("옵션")]
        [SerializeField] private bool loadOnEnable = true;

        public override string LocaleKey => key;
        public override int FontIndex => fontId;
        public override bool LoadOnEnable => loadOnEnable;
        
        private void OnEnable()
        {
            if (loadOnEnable) 
                UpdateFont();
        }
        
        public override void OnLocaleUpdated(string localizedString)
        {
            tmp ??= typewriter?.GetComponent<TMP_Text>();
            if (!tmp || !typewriter) return;
            
            // 폰트 업데이트
            UpdateFont();
            
            // 텍스트 업데이트
            // typewriter.TextAnimator.SwapText(localizedString);
            // if (typewriter.isShowingText)
            // {
            //     //typewriter.TextAnimator.SwapText();
            //     //typewriter.SkipTypewriter();
            // }
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
            typewriter = GetComponent<TypewriterCore>();
            tmp ??= typewriter?.GetComponent<TMP_Text>();
        }
    }
}