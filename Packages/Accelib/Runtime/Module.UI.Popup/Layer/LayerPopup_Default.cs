using Accelib.Module.UI.Popup.Layer.Base;
using UnityEngine;

namespace Accelib.Module.UI.Popup.Layer
{
    public class LayerPopup_Default : LayerPopupBase
    {
        [SerializeField] private bool allowMultiInstance = false;
        [SerializeField] private bool hideOnLostFocus = true;

        public override bool AllowMultiInstance => allowMultiInstance;
        public override bool HideOnLostFocus => hideOnLostFocus;
    }
}