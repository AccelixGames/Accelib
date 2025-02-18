using System.Collections.Generic;
using Accelib.Extensions;
using Accelib.Logging;
using Accelib.Module.UI.InfoBox.Base.Control.Provider.Interface;
using Accelib.Module.UI.InfoBox.Base.Model;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Accelib.Module.UI.InfoBox.Base.Control.Provider
{
    public class InfoBroadcaster : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Config")]
        [SerializeField] private InfoDataBaseEvent onInformed;
        [SerializeField] private bool pointerExitOnDisable = false;

        [Header("Interface info")]
        [SerializeField, ReadOnly] private bool hasProvider;
        private _IInfoProvider _provider;

        [Header("Debug")]
        [SerializeField] private bool debugLog = false;

        private void OnEnable()
        {
            _provider = GetComponent<_IInfoProvider>();
            hasProvider = _provider is not null;
        }

        private void OnDisable()
        {
            if (pointerExitOnDisable)
            {
                onInformed?.Raise(null);
                Log("null : OnDisable");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_provider is null) return;
            
            onInformed?.Raise(_provider.Provide());
            Log("info : OnPointerEnter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onInformed?.Raise(null);
            Log("null : OnPointerExit");
        }

        private void Log(string msg)
        {
            if(debugLog)
                Deb.Log(msg, this);
        }

#if UNITY_EDITOR
        private void Reset() => OnEnable();
        private void OnValidate() => OnEnable();
#endif
    }
}