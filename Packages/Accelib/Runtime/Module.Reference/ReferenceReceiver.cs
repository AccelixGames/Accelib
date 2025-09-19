using System;
using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Module.Reference
{
    [DefaultExecutionOrder(100)]
    public class ReferenceReceiver<T> : MonoBehaviour where T : class
    {
        [Header("# 레퍼런스")]
        [SerializeField] private SOReference reference;
        protected List<T> References;

        protected virtual void OnEnable()
        {
            References = reference?.FindByType<T>();
        }
    }
}