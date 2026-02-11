using Accelib.Logging;
using Accelib.Module.UI.InfoBox.Base.Control.Provider.Interface;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Accelib.Module.UI.InfoBox.Base.Control.Provider
{
    public class InfoBroadcaster : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Title("옵션")]
        [SerializeField] private InfoDataBaseEvent onInformed;
        [SerializeField] private bool pointerExitOnDisable = false;
        [SerializeField] private bool debugLog = false;

        [Title("디버그")]
        [ShowInInspector, ReadOnly] private _IInfoProvider _provider;

        private void OnEnable()
        {
            _provider = GetComponent<_IInfoProvider>();
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