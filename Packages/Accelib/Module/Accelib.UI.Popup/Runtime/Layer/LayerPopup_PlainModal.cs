using Accelib.Module.UI.Popup.Data;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Accelib.Module.UI.Popup.Layer
{
    /// <summary>
    /// 일반 텍스트 모달 다이얼로그.
    /// </summary>
    public sealed class LayerPopup_PlainModal : LayerPopup_Modal
    {
        [TitleGroup("텍스트")]
        [SerializeField] private TMP_Text titleTMP;
        [TitleGroup("텍스트")]
        [SerializeField] private TMP_Text descTMP;
        [TitleGroup("버튼")]
        [SerializeField] private TMP_Text okButton;
        [TitleGroup("버튼")]
        [SerializeField] private TMP_Text ngButton;

        protected override void ApplyOption(ModalOpenOption option)
        {
            titleTMP.text = option.title;
            descTMP.text = option.desc;

            if (okButtonObj && okButtonObj.activeSelf) okButton.text = option.ok;
            if (ngButtonObj && ngButtonObj.activeSelf) ngButton.text = option.ng;
        }
    }
}
