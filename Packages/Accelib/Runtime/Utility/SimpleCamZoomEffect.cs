using System;
using Accelib.Data;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Utility
{
    public class SimpleCamZoomEffect : MonoBehaviour
    {
        private enum FadeMode { In, Out, None }
        private enum CalcMethod {Relative = 0, Absolute}

        [SerializeField] private Camera cam;
        [SerializeField] private EasePairTweenConfig config;

        [Header("Zoom")]
        [SerializeField] private bool isIndependentUpdate = false;
        [SerializeField] private CalcMethod calcMethod = CalcMethod.Relative;
        [SerializeField] private float zoomInAmount = 2f;
        [SerializeField, ReadOnly] private float initZoomAmount = 2f;
        
        private Tweener _tween;

        private void OnEnable() => initZoomAmount = cam.orthographicSize;
        private void OnDisable() => _tween?.Kill(true);

        public Tweener DoZoomIn() => Internal_Zoom(true);
        public Tweener DoZoomOut() => Internal_Zoom(false);
        
        [Button, EnableIf("@UnityEngine.Application.isPlaying")]
        public void ZoomIn() => DoZoomIn();
        [Button, EnableIf("@UnityEngine.Application.isPlaying")]
        public void ZoomOut() => DoZoomOut();

        private Tweener Internal_Zoom(bool zoomIn)
        {
            if(zoomIn) initZoomAmount = cam.orthographicSize;
            
            var inValue = calcMethod == CalcMethod.Relative ? initZoomAmount + zoomInAmount : zoomInAmount;
            var outValue = initZoomAmount;
            var value = zoomIn ? inValue : outValue;
            
            _tween?.Kill(true);
            _tween = cam.DOOrthoSize(value, config.GetDuration(zoomIn))
                .SetEase(config.GetEase(zoomIn)).SetUpdate(isIndependentUpdate);
           
            var scaleController = cam.GetComponent<AspectScaleController>();
            if (scaleController)
            {
                if(zoomIn) _tween.OnStart(() => scaleController.IsLocked = true);
                if(!zoomIn) _tween.OnComplete(() => scaleController.IsLocked = false);
            }
            
            return _tween;
        }
        
        private void Reset()
        {
            cam = GetComponent<Camera>();
        }
    }
}