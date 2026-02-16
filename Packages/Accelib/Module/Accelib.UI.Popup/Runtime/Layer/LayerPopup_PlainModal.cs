using Accelib.Module.UI.Popup.Data;
using TMPro;
using UnityEngine;

namespace Accelib.Module.UI.Popup.Layer
{
    /// <summary>
    /// 일반 텍스트 모달 다이얼로그.
    /// </summary>
    public sealed class LayerPopup_PlainModal : LayerPopup_Modal
    {
        [SerializeField] private TMP_Text titleTMP;
        [SerializeField] private TMP_Text descTMP;
        [SerializeField] private TMP_Text okButton;
        [SerializeField] private GameObject okButtonObj;
        [SerializeField] private TMP_Text ngButton;
        [SerializeField] private GameObject ngButtonObj;

        protected override void ApplyOption(ModalOpenOption option)
        {
            SetText(titleTMP, option.title, null);
            SetText(descTMP, option.desc, null);
            SetText(okButton, option.ok, okButtonObj);
            SetText(ngButton, option.ng, ngButtonObj);
        }

        private static void SetText(TMP_Text target, string text, GameObject obj)
        {
            if (obj)
            {
                obj.SetActive(!string.IsNullOrEmpty(text));
                if (!obj.activeSelf) return;
            }

            target.text = text;
        }
    }
}
