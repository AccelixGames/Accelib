using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Accelib.Utility
{
    [RequireComponent(typeof(Camera))]
    public class AspectScaleController : MonoBehaviour
    {
        [Header("")]
        [SerializeField] private RectTransform canvas;
        [SerializeField] private bool autoUpdate = false; 
        
        [Header("")]
        [SerializeField, Range(0f, 4096f)] private float referenceWidth = 1920;
        [SerializeField, Range(0f, 4096f)] private float referenceHeight = 568;
        [SerializeField, MinMaxSlider(0f, 300f)] private Vector2 fovRange = new(30f, 60f);
        
        [Header("Size Debug")]
        [SerializeField, ReadOnly] private Camera cam;
        [SerializeField, ReadOnly] private float initOrthographicSize = 11.5f;
        [SerializeField, ReadOnly] private float initFieldOfView = 60f;
        [SerializeField, ReadOnly] private float ratio;

        
        private void Start()
        {
            cam = GetComponent<Camera>();
            initOrthographicSize = cam.orthographicSize;
            initFieldOfView = cam.fieldOfView;
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
            if (cam.orthographic)
            {
                var newRatio = 1f / Mathf.Min(referenceHeight / canvas.sizeDelta.y, 1f);
                if (Mathf.Abs(ratio - newRatio) < 0.001f) return;
            
                ratio = newRatio;
                cam.orthographicSize = initOrthographicSize * ratio;
            }
            else
            {
                var newRatio = Mathf.Min(referenceWidth / canvas.sizeDelta.x, 1f);
                if (Mathf.Abs(ratio - newRatio) < 0.001f) return;
            
                ratio = newRatio;
                cam.fieldOfView = Mathf.Clamp(initFieldOfView * ratio, fovRange.x, fovRange.y);
            }
        }
    }
}