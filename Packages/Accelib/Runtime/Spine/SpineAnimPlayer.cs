#if ACCELIX_SPINE
using System;
using System.Collections.Generic;
using Accelib.Spine.Architecture;
using NaughtyAttributes;
using Spine.Unity;
using UnityEngine;

namespace Accelib.Spine
{
    //[RequireComponent(typeof(SkeletonAnimation))]
    public class SpineAnimPlayer : MonoBehaviour
    {
        [SerializeField] private List<SpineAnimationGroup> animations;

        private SkeletonAnimation _anim;
        private SkeletonGraphic _graphic;

        private void Awake()
        {
            _anim = GetComponent<SkeletonAnimation>();
            _graphic = GetComponent<SkeletonGraphic>();
            if (_anim == null && _graphic == null)
            {
                Debug.LogError("SkeletonAnimation이 없습니다.", this);
                Destroy(this);
            }
        }
        
        private  void OnEnable()
        {
            foreach (var anim in animations)
            {
                if (anim.method == SpineAnimMethod.Add)
                {
                    _anim?.AnimationState.AddAnimation(anim.trackId, anim.animName, anim.loop, anim.delay);
                    _graphic?.AnimationState.AddAnimation(anim.trackId, anim.animName, anim.loop, anim.delay);
                }
                else
                {
                    _anim?.AnimationState.SetAnimation(anim.trackId, anim.animName, anim.loop);
                    _graphic?.AnimationState.SetAnimation(anim.trackId, anim.animName, anim.loop);
                }
            }
        }
    }
}
#endif