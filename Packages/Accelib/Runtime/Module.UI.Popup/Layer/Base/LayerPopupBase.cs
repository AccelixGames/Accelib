﻿using Accelib.Logging;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.UI.Popup.Layer.Base
{
    public abstract class LayerPopupBase : MonoBehaviour
    {
        public virtual bool AllowMultiInstance => false;
        public virtual bool HideOnLostFocus => true;

        internal void OpenLayer(object param)
        {
            OnPreOpen(param);
            gameObject.SetActive(true);
            OnPostOpen(param);
        }

        protected virtual void OnPreOpen(object param) {}
        protected virtual void OnPostOpen(object param) {}
        internal virtual void OnClose() {}
        internal virtual void OnLostFocus() {}
        internal virtual void OnRegainFocus() {}

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void Open()
        {
            if(!(PopupSingleton.Instance?.OpenLayer(this) ?? false))
                Deb.LogWarning($"팝업을 열지 못했습니다: {name}", this);
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void Close()
        {
            if(!(PopupSingleton.Instance?.CloseLayer(this) ?? false))
                Deb.LogWarning($"팝업을 닫지 못했습니다: {name}", this);
        }
    }
}