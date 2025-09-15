using System;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Utility
{
    public class SimpleTraceTarget : MonoBehaviour
    {
        public enum UpdateMode { Update, LateUpdate, FixedUpdate }
        
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Option")]
        [SerializeField] private UpdateMode mode;
        [SerializeField] private Vector3 pivot;
        
        [Header("Lock")]
        [SerializeField] private bool lockX;
        [SerializeField] private bool lockY;
        [SerializeField] private bool lockZ;

        [Header("UseMinMax")]
        [SerializeField] private bool useMinMax = false;
        [SerializeField, ShowIf(nameof(useMinMax))] private Vector3 min;
        [SerializeField, ShowIf(nameof(useMinMax))] private Vector3 max;

        public void SetMinMax(Vector3 _min, Vector3 _max)
        {
            min = _min;
            max = _max;
        }
        
        private void Trace()
        {
            var pos = transform.position;
            var tPos = target.position + pivot;

            if (!lockX) pos.x = useMinMax ? Mathf.Clamp(tPos.x, min.x, max.x) : tPos.x;
            if (!lockY) pos.y = useMinMax ? Mathf.Clamp(tPos.y, min.y, max.y) : tPos.y;
            if (!lockZ) pos.z = useMinMax ? Mathf.Clamp(tPos.z, min.z, max.z) : tPos.z;

            transform.position = pos;
        }

        private void Rotate()
        {
            transform.rotation = target.rotation;
        }

        private void Update()
        {
            if (mode == UpdateMode.Update) Trace();
        }

        private void LateUpdate()
        {
            if (mode == UpdateMode.LateUpdate) Trace();
        }

        private void FixedUpdate()
        {
            if (mode == UpdateMode.FixedUpdate) Trace();
        }
    }
}