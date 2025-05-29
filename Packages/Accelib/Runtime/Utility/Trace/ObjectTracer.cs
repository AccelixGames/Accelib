using System;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Utility.Trace
{
    public class ObjectTracer : MonoBehaviour
    {
        [SerializeField] private FindObjectsInactive findObjectsInactive;
        
        [SerializeField, ReadOnly] private ObjectTraceTarget target;

        private void OnEnable()
        {
            target = FindFirstObjectByType<ObjectTraceTarget>(findObjectsInactive);
        }

        private void Update()
        {
            transform.position = target.transform.position;
        }
    }
}