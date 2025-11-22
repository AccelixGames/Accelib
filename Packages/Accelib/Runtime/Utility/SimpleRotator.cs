using System;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Utility
{
    public class SimpleRotator : MonoBehaviour
    {
        [SerializeField] private float spd;
        [SerializeField] private float accel;
        [SerializeField] private Vector3 rotateNormal;

        [Header("# Option")]
        [SerializeField] private bool preserveChildLocalRotation;
        [SerializeField] private Transform child;
        
        [Header("# Curr")]
        [SerializeField, ReadOnly] private bool isEnabled;
        [SerializeField, ReadOnly] private float currSpd;

        public void Toggle(bool enable) => isEnabled = enable;
        
        private void OnEnable()
        {
            rotateNormal = rotateNormal.normalized;
            
            isEnabled = false;
            currSpd = 0f;
        }

        private void Update()
        {
            if (isEnabled)
                currSpd += accel * Time.deltaTime;
            else
                currSpd -= accel * Time.deltaTime;
            
            currSpd = Mathf.Clamp(currSpd, 0f, spd);

            var rot = Quaternion.identity;
            if (preserveChildLocalRotation && child)
                rot = child.rotation;
            
            transform.Rotate( rotateNormal * (currSpd * Time.deltaTime));
            
            if (preserveChildLocalRotation && child)
                child.rotation = rot;
        }
    }
}