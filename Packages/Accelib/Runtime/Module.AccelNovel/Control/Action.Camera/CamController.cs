using Accelib.Data;
using Accelib.Helper;
using Accelib.Tween;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Accelib.Module.AccelNovel.Control.Action.Camera
{
    public sealed class CamController : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private Transform pivotForIdleMotion;
        [SerializeField] private Transform pivotForZoom;
        [SerializeField] private UnityEngine.Camera cam;

        [Header("Option")]
        [SerializeField] private IntVariable screenShakeMode; 
        
        [Header("Idle Motion")]
        [SerializeField] private Transform idleMotionDestination;
        [SerializeField] private DefaultTweenValue idleMotionTweenValue;
        [SerializeField] private Timer destinationTimer;
        [SerializeField, Range(0f,2f)] private float moveRadiusX;
        [SerializeField, Range(0f,2f)] private float moveRadiusY;
        
        [Header("Zoom")]
        [SerializeField] private EasePairTweenConfig zoomConfig;
        [SerializeField] private Transform zoomOutPos;
        [SerializeField] private Transform zoomInPos;

        private Tweener _idleTween;

        private void GetRandomIdlePos()
        {
            var pos = Random.insideUnitCircle;
            pos.x *= moveRadiusX;
            pos.y *= moveRadiusY;
            idleMotionDestination.position = pos;
        }
        
        private void Start()
        {
            GetRandomIdlePos();
            
            destinationTimer.Set(idleMotionTweenValue.duration);
            _idleTween = pivotForIdleMotion.DOMove(idleMotionDestination.position, idleMotionTweenValue.duration)
                .SetEase(idleMotionTweenValue.ease)
                .SetLink(pivotForIdleMotion.gameObject)
                .SetAutoKill(false);
            
            screenShakeMode.Changed.Register(OnScreenShakeModeChanged);
            OnScreenShakeModeChanged(screenShakeMode.Value);
        }
        
        private void OnDestroy()
        {
            _idleTween?.Kill();
            screenShakeMode.Changed.Unregister(OnScreenShakeModeChanged);
        }

        private void OnScreenShakeModeChanged(int option)
        {
            if (option == 0)
            {
                GetRandomIdlePos();
                _idleTween?.ChangeEndValue(idleMotionDestination.position, true).Restart();
                destinationTimer.Set(idleMotionTweenValue.duration);
            }
            else
            {
                _idleTween?.Pause();
            }
        }

        private void Update()
        {
            if(screenShakeMode.Value != 0) return;
            
            if (destinationTimer.OnTime())
            {
                GetRandomIdlePos();
                _idleTween.ChangeEndValue(idleMotionDestination.position, true).Restart();
                destinationTimer.Set(idleMotionTweenValue.duration);
            }
        }

        // public Tween DoZoomIn(bool zoomIn)
        // {
        //     var targetPos = zoomIn ? zoomInPos.localPosition: zoomOutPos.localPosition;
        //     var duration = zoomConfig.duration;
        //     var ease = zoomIn ? zoomConfig.easeA : zoomConfig.easeB;
        //
        //     return pivotForZoom
        //         .DOLocalMove(targetPos, duration)
        //         .SetEase(ease)
        //         .SetLink(pivotForZoom.gameObject);
        // }
        //
        //   
        // public Tweener DoShake(ShakeTweenConfig tween, bool lockX = false, bool lockY = false)
        // {
        //     var strength = tween.strength;
        //     if (lockX) strength.x = 0f;
        //     if (lockY) strength.y = 0f;
        //     strength.z = 0f;
        //
        //     return cam
        //         .DOShakePosition(tween.duration, strength, tween.vibrato, tween.randomness, tween.fadeOut)
        //         .SetLink(cam.gameObject);
        // }
        
        public Tweener DoShake(float duration, Vector3 str, int vibrato, float randomness, bool fadeOut)
        {
            return cam
                .DOShakePosition(duration, str, vibrato, randomness, fadeOut)
                .SetLink(cam.gameObject);
        }
    }
}