using System;
using System.Collections.Generic;
using Accelib.Extensions;
using Accelib.Module.UI.InfoBox.Base.Model;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Accelib.Module.UI.InfoBox.Base.Control
{
    public class InfoBroadcaster : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, ReadOnly] private int providerCount = 0;
        [SerializeField] private bool pointerExitOnDisable = false;
        
        private _IInfoProvider[] _providers;

        private void OnEnable()
        {
            _providers = GetComponents<_IInfoProvider>();
            providerCount = _providers?.Length ?? 0;
        }

        private void OnDisable()
        {
            if(pointerExitOnDisable)
                InfoBoxControllerSingleton.DrawInfo(null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            var infos = new List<InfoDataBase>();
            foreach (var provider in _providers)
            {
                infos.AddNullCheck(provider.GetInfo());
                infos.AddRangeNullCheck(provider.GetInfoList());
            }

            InfoBoxControllerSingleton.DrawInfoList(infos);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InfoBoxControllerSingleton.DrawInfo(null);
        }

        #if UNITY_EDITOR
        private void Reset() => OnEnable();
        private void OnValidate() => OnEnable();
        #endif
    }
}