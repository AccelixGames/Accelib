using Accelib.Module.Localization.Helper;
using Accelib.Module.UI.Popup.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Module.UI.Popup.Layer
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

        protected override void ApplyOption(ModalOpenOption option)
        {
            titleTMP.ChangeKey(option.title);
            descTMP.ChangeKey(option.desc, option.descParams);

            if (okButtonObj && okButtonObj.activeSelf) okButton.ChangeKey(option.ok);
            if (ngButtonObj && ngButtonObj.activeSelf) ngButton.ChangeKey(option.ng);
        }
    }
}
