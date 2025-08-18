using System;
using UnityEngine;

namespace Accelib.Module.UI.Utility
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;

        private void OnEnable()
        {
            if(targetCamera == null)
                targetCamera = Camera.main;
        }

        private void LateUpdate()
        {
            transform.rotation = targetCamera.transform.rotation;
        }
    }
}