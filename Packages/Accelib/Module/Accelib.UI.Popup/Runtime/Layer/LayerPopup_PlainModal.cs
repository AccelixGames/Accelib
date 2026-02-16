using Accelib.UI.Popup.Runtime.Data;
using Accelib.UI.Popup.Runtime.Layer.Base;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Accelib.UI.Popup.Runtime.Layer
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

        protected override void ApplyOption(IModalOptionProvider option)
        {
            titleTMP.text = option.Title;
            descTMP.text = option.Desc;

            if (okButtonObj && okButtonObj.activeSelf) okButton.text = option.Ok;
            if (ngButtonObj && ngButtonObj.activeSelf) ngButton.text = option.Ng;
        }
    }
}
