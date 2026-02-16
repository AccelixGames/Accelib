using Accelib.Module.Localization.Helper;
using Accelib.Module.UI.Popup.Data;
using UnityEngine;

namespace Accelib.Module.UI.Popup.Layer
{
    /// <summary>
    /// 로컬라이제이션 키 기반 모달 다이얼로그.
    /// </summary>
    public sealed class LayerPopup_LocalizedModal : LayerPopup_Modal
    {
        [SerializeField] private LocalizedTMP titleTMP;
        [SerializeField] private LocalizedTMP descTMP;
        [SerializeField] private LocalizedTMP okButton;
        [SerializeField] private GameObject okButtonObj;
        [SerializeField] private LocalizedTMP ngButton;
        [SerializeField] private GameObject ngButtonObj;

        protected override void ApplyOption(ModalOpenOption option)
        {
            SetText(titleTMP, option.title, null);
            SetText(descTMP, option.desc, null, option.descParams);
            SetText(okButton, option.ok, okButtonObj);
            SetText(ngButton, option.ng, ngButtonObj);
        }

        private static void SetText(LocalizedTMP target, string text, GameObject obj, params object[] args)
        {
            if (obj)
            {
                obj.SetActive(!string.IsNullOrEmpty(text));
                if (!obj.activeSelf) return;
            }

            target.ChangeKey(text, args);
        }
    }
}
