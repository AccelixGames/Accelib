#if ACCELIX_SPINE
using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Spine.Unity;
using UnityEngine;

namespace Accelib.Spine
{
    //[RequireComponent(typeof(SkeletonAnimation))]
    public class SpineAnimPlayer : MonoBehaviour
    {
        [SerializeField] private List<AnimationGroup> animations;

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
                if (anim.method == AnimationGroup.Method.Add)
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

        [Serializable]
        private class AnimationGroup
        {
            public enum Method {Set, Add}
            
            public Method method = Method.Set;
            [Range(0, 5)] public int trackId = 0;
            [SpineAnimation] public string animName;
            public bool loop = false;
            [ShowIf(nameof(method), Method.Add), Range(0f, 5f)]
            public float delay = 0f;
        }
    }
}
#endif