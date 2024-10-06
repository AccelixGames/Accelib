#if ACCELIX_SPINE
using System;
using Accelib.Logging;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using Event = Spine.Event;

namespace Accelib.Spine
{
    public class SimpleSpineEvent : MonoBehaviour
    {
        [System.Serializable]
        private class EventGroup
        {
            [SpineEvent(dataField = nameof(animator))] public string name;
            public UnityEvent onEvent;
        }
        
        [System.Serializable]
        private class AnimGroup
        {
            [SpineAnimation(dataField = nameof(animator))] public string name;
            public UnityEvent onEvent;
        }
        
        [Header("Target")]
        [SerializeField] private SkeletonAnimation animator;

        [Header("Event")]
        [Tooltip("애니메이션 재생이 시작될 때 (캔슬 포함)")]
        [SerializeField] private AnimGroup[] onStart;
        [Tooltip("애니메이션 재생이 끝날 때 (캔슬 포함)")]
        [SerializeField] private AnimGroup[] onEnd;
        [Tooltip("애니메이션 재생이 완전히 끝날 때 (캔슬 미포함)")]
        [SerializeField] private AnimGroup[] onComplete;
        [Tooltip("커스텀 이벤트 발생 시")]
        [SerializeField] private EventGroup[] onEvent;

        private void OnEnable()
        {
            if (animator == null) return;
            
            animator.AnimationState.Start += OnStart; 
            animator.AnimationState.End += OnEnd;
            animator.AnimationState.Complete += OnComplete;
            animator.AnimationState.Event += OnSpineEvent;
        }
        
        private void OnDisable()
        {
            if (animator == null) return;
            
            animator.AnimationState.Start -= OnStart; 
            animator.AnimationState.End -= OnEnd;
            animator.AnimationState.Complete -= OnComplete;
            animator.AnimationState.Event -= OnSpineEvent;
        }

        private void OnStart(TrackEntry trackEntry) => InvokeAll(onStart, trackEntry.Animation.Name);

        private void OnEnd(TrackEntry trackEntry) => InvokeAll(onEnd, trackEntry.Animation.Name);
        
        private void OnComplete(TrackEntry trackEntry) => InvokeAll(onComplete, trackEntry.Animation.Name);

        private void OnSpineEvent(TrackEntry trackEntry, Event e)
        {
            foreach (var eGroup in onEvent)
            {
                if(e.Data.Name == eGroup.name)
                    eGroup.onEvent?.Invoke();
            }
        }
        
        private void Reset() => animator ??= GetComponent<SkeletonAnimation>();

        private void InvokeAll(AnimGroup[] groups, string key)
        {
            foreach (var group in groups)
                if(key == group.name)
                    group.onEvent?.Invoke();
        }
    }
}
#endif