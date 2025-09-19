using System;
using UnityEngine;

namespace Accelib.Module.Reference
{
    [DefaultExecutionOrder(-100)]
    public class ReferenceProvider : MonoBehaviour
    {
        [Header("# 레퍼런스")]
        [SerializeField] private SOReference reference;

        private void OnEnable()
        {
            if (reference == null) return;
            reference.Register(gameObject);
        }

        private void OnDisable()
        {
            if (reference == null) return;
            reference.UnRegister(gameObject);
        }
    }
}