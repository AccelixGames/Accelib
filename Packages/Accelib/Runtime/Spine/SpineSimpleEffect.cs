﻿#if ACCELIX_SPINE
using System;
using Spine;
using Spine.Unity;
using UnityEngine;
using Event = Spine.Event;

namespace Accelib.Spine
{
    public class SpineSimpleEffect : MonoBehaviour
    {
        private enum EndMode {Loop, Disable, Destroy}
        
        [SerializeField] private SkeletonAnimation skeletonAnim;
        [SerializeField, SpineAnimation] private string defaultAnim;
        [SerializeField] private EndMode endMode;

        private void Awake()
        {
            if (endMode != EndMode.Loop) 
                skeletonAnim.AnimationState.Complete += OnEnd;
        }

        private void OnEnd(TrackEntry trackEntry)
        {
            if(endMode == EndMode.Disable)
                gameObject.SetActive(false);
            else if(endMode == EndMode.Destroy)
                Destroy(gameObject);
        }
    }
}
#endif