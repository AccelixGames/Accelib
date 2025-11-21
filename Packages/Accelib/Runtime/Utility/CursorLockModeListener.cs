using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Utility
{
    public class CursorLockModeListener : MonoBehaviour
    {
        [SerializeField] private UnityEvent<CursorLockMode> onCursorLockModeChanged;
        [SerializeField] private UnityEvent onCursorLockModeNone;
        [SerializeField] private UnityEvent onCursorLockModeLocked;
        [SerializeField] private UnityEvent onCursorLockModeConfined;

        [Header("Debug")]
        [SerializeField, ReadOnly] private CursorLockMode prevLockMode;

        private void OnEnable()
        {
            prevLockMode = Cursor.lockState;
        }

        private void LateUpdate()
        {
            var currLockMode = Cursor.lockState;
            if (prevLockMode == currLockMode) return;

            prevLockMode = currLockMode;
            onCursorLockModeChanged?.Invoke(currLockMode);
            
            if(currLockMode == CursorLockMode.None)
                onCursorLockModeNone?.Invoke();
            else if(currLockMode == CursorLockMode.Locked)
                onCursorLockModeLocked?.Invoke();
            else if(currLockMode == CursorLockMode.Confined)
                onCursorLockModeConfined?.Invoke();
        }
    }
}