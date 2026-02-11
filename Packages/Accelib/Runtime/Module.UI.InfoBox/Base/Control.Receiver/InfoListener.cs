using System;
using Accelib.Module.UI.InfoBox.Base.Control.Receiver.Interface;
using Accelib.Module.UI.InfoBox.Base.Model;
using Sirenix.OdinInspector;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Module.UI.InfoBox.Base.Control.Receiver
{
    public class InfoListener : MonoBehaviour
    {
        [Title("Config")]
        [SerializeField] private InfoDataBaseEvent onInformed;
        
        [Title("Interface info")]
        [ShowInInspector, ReadOnly] private _IInfoReceiver _receiver;
        
        private void OnEnable()
        {
            _receiver = GetComponent<_IInfoReceiver>();
            
            onInformed?.Register(OnInformed);
        }

        private void OnDisable() => onInformed?.Unregister(OnInformed);

        private void OnInformed(InfoDataBase info)
        {
            if(this == null || gameObject == null || !gameObject.activeInHierarchy) return;
            
            _receiver?.Receive(info);
        }

#if UNITY_EDITOR
        private void Reset() => OnEnable();
        private void OnValidate() => OnEnable();
#endif
    }
}