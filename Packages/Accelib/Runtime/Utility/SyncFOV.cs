using System;
using UnityEngine;

namespace Accelib.Utility
{
    public class SyncFOV: MonoBehaviour
    {
        [SerializeField] private Camera from;
        [SerializeField] private Camera to;

        private void LateUpdate() => to.fieldOfView = from.fieldOfView;

        private void Reset()
        {
            from = Camera.main;
            to = GetComponent<Camera>();
        }
    }
}