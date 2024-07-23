using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Accelib.Utility
{
    [RequireComponent(typeof(Camera))]
    public class AspectScaleController : MonoBehaviour
    {
        [FormerlySerializedAs("target")]
        [SerializeField] private RectTransform canvas;
        [SerializeField, Range(0f, 2048f)] private float referenceHeight = 568;
        [SerializeField] private bool autoUpdate = false; 
        
        [Header("Size Debug")]
        [SerializeField, ReadOnly] private float initialSize = 11.5f;
        [SerializeField, ReadOnly] private float ratio;
        [SerializeField, ReadOnly] private Camera cam;
        
        private void Start()
        {
            cam = GetComponent<Camera>();
            initialSize = cam.orthographicSize;
            ratio = -1f;

            UpdateRatio();
        }

        private void LateUpdate()
        {
            if (autoUpdate) 
                UpdateRatio();
        }

        private void UpdateRatio()
        {
            var newRatio = 1f / Mathf.Min(referenceHeight / canvas.sizeDelta.y, 1f);
            if (Mathf.Abs(ratio - newRatio) < 0.001f) return;
            
            ratio = newRatio;
            cam.orthographicSize = initialSize * ratio;
        }
    }
}