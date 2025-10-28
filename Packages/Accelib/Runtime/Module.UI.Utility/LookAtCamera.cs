using System;
using UnityEngine;

namespace Accelib.Module.UI.Utility
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;

        private void OnEnable()
        {
            if (!targetCamera) targetCamera = Camera.main;
        }

        private void LateUpdate()
        {
            if (!targetCamera) targetCamera = Camera.main;
            if (!targetCamera) return;
            
            transform.rotation = targetCamera.transform.rotation;
        }
    }
}