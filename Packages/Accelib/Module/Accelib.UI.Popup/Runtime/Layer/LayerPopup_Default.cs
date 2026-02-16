using Accelib.UI.Popup.Runtime.Layer.Base;
using UnityEngine;

namespace Accelib.UI.Popup.Runtime.Layer
{
    public class LayerPopup_Default : LayerPopupBase
    {
        [SerializeField] private string popupID;
        [SerializeField] private bool allowMultiInstance = false;
        [SerializeField] private bool hideOnLostFocus = true;

        public override bool AllowMultiInstance => allowMultiInstance;
        public override bool HideOnLostFocus => hideOnLostFocus;

        public override string GetId() => popupID;
    }
}
