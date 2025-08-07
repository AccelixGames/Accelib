using System;
using Accelib.Logging;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Extension.Atom
{
    public class BoolEventListener : MonoBehaviour
    {
        [SerializeField] private BoolVariable variable;
        
        [Header("True/False")]
        [SerializeField] private UnityEvent onTrue;
        [SerializeField] private UnityEvent onFalse;
        
        [Header("Bool")]
        [SerializeField] private UnityEvent<bool> onEvent;
        [SerializeField] private bool reverseValue = false;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool log = false;
#endif

        private void OnEnable() => variable.Changed.Register(OnEventRaised);
        private void OnDisable() => variable.Changed.Unregister(OnEventRaised);

        private void OnEventRaised(bool value)
        {
            try
            {
                if (value)
                    onTrue?.Invoke();
                else
                    onFalse?.Invoke();

                onEvent?.Invoke(reverseValue ? !value : value);

#if UNITY_EDITOR
                if(variable && log)
                    Debug.Log($"{variable.name}: {variable.Value}");
#endif
            }
            catch (Exception e)
            {
                Deb.LogException(e, this);
            }
        }
    }
}