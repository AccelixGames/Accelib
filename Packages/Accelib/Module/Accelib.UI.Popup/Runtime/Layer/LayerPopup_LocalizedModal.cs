using Accelib.Module.Localization.Helper;
using Accelib.UI.Popup.Runtime.Data;
using Accelib.UI.Popup.Runtime.Layer.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.UI.Popup.Runtime.Layer
{
    /// <summary>
    /// 로컬라이제이션 키 기반 모달 다이얼로그.
    /// </summary>
    public sealed class LayerPopup_LocalizedModal : LayerPopup_Modal
    {
        [TitleGroup("텍스트")]
        [SerializeField] private LocalizedTMP titleTMP;
        [TitleGroup("텍스트")]
        [SerializeField] private LocalizedTMP descTMP;
        [TitleGroup("버튼")]
        [SerializeField] private LocalizedTMP okButton;
        [TitleGroup("버튼")]
        [SerializeField] private LocalizedTMP ngButton;

        protected override void ApplyOption(IModalOptionProvider option)
        {
            titleTMP.ChangeKey(option.Title);
            descTMP.ChangeKey(option.Desc, option.DescParams);

            if (okButtonObj && okButtonObj.activeSelf) okButton.ChangeKey(option.Ok);
            if (ngButtonObj && ngButtonObj.activeSelf) ngButton.ChangeKey(option.Ng);
        }
    }
}
