#if ACCELIX_SPINE
using System;
using Accelib.Logging;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using AnimationState = Spine.AnimationState;
using Event = Spine.Event;

namespace Accelib.Spine
{
    public class SimpleSpineEventV2 : MonoBehaviour
    {
        [System.Serializable]
        private class EventGroup
        {
            [SpineEvent(dataField = nameof(dataAsset))] public string name;
            public UnityEvent onEvent;
        }
        
        [System.Serializable]
        private class AnimGroup
        {
            [SpineAnimation(dataField = nameof(dataAsset))] public string name;
            public UnityEvent onEvent;
        }

        [Header("Target")]
        [SerializeField] private SkeletonDataAsset dataAsset;
        [SerializeField] private SkeletonAnimation animator;
        [SerializeField] private SkeletonGraphic graphic;

        [Header("Event")]
        [Tooltip("애니메이션 재생이 시작될 때 (캔슬 포함)")]
        [SerializeField] private AnimGroup[] onStart;
        [SerializeField] private AnimGroup[] onInterrupt;
        [Tooltip("애니메이션 재생이 끝날 때 (캔슬 포함)")]
        [SerializeField] private AnimGroup[] onEnd;
        [Tooltip("애니메이션 재생이 완전히 끝날 때 (캔슬 미포함)")]
        [SerializeField] private AnimGroup[] onComplete;
        [Tooltip("커스텀 이벤트 발생 시")]
        [SerializeField] private EventGroup[] onEvent;
        
        [Header("Log")]
        [SerializeField] private bool showLog = false;

        private AnimationState _animState;

        private void Awake()
        {
            _animState = animator?.AnimationState ?? graphic?.AnimationState;
        }

        private void OnEnable()
        {
            if (_animState == null) return;
            
            _animState.Start += OnStart;
            _animState.Interrupt += OnInterrupt;
            _animState.End += OnEnd;
            _animState.Complete += OnComplete;
            _animState.Event += OnSpineEvent;
        }

        private void OnDisable()
        {
            if (_animState == null) return;
            
            _animState.Start -= OnStart; 
            _animState.Interrupt -= OnInterrupt;
            _animState.End -= OnEnd;
            _animState.Complete -= OnComplete;
            _animState.Event -= OnSpineEvent;
        }

        private void OnStart(TrackEntry trackEntry) => InvokeAll(onStart, trackEntry.Animation.Name, "OnStart");
        private void OnInterrupt(TrackEntry trackEntry) => InvokeAll(onInterrupt, trackEntry.Animation.Name, "OnInterrupt");
        private void OnEnd(TrackEntry trackEntry) => InvokeAll(onEnd, trackEntry.Animation.Name, "OnEnd");
        private void OnComplete(TrackEntry trackEntry) => InvokeAll(onComplete, trackEntry.Animation.Name, "OnComplete");
        private void OnSpineEvent(TrackEntry trackEntry, Event e)
        {
            foreach (var eGroup in onEvent)
            {
                if(e.Data.Name == eGroup.name)
                    eGroup.onEvent?.Invoke();
            }
        }
        
        private void Reset()
        {
            animator ??= GetComponent<SkeletonAnimation>();
            graphic ??= GetComponent<SkeletonGraphic>();

            dataAsset = animator?.SkeletonDataAsset ?? graphic?.SkeletonDataAsset;
        }

        private void InvokeAll(AnimGroup[] groups, string key, string msg)
        {
            if(showLog && !string.IsNullOrEmpty(msg))
                Deb.Log(msg + $": {key}", this);
            
            foreach (var group in groups)
                if(key == group.name)
                    group.onEvent?.Invoke();
        }
    }
}
#endif