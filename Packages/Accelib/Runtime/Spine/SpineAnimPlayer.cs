using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Spine.Unity;
using UnityEngine;

namespace Accelib.Spine
{
    public class SpineAnimPlayer : MonoBehaviour
    {
        [SerializeField] private List<AnimationGroup> animations;

        private SkeletonAnimation skeletonAnimation;
        
        private void Awake()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            if (skeletonAnimation == null)
            {
                Debug.LogError("SkeletonAnimation이 없습니다.", this);
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            foreach (var anim in animations)
            {
                if (anim.method == AnimationGroup.Method.Add)
                    skeletonAnimation.AnimationState.AddAnimation(anim.trackId, anim.animName, anim.loop, anim.delay);
                else
                    skeletonAnimation.AnimationState.SetAnimation(anim.trackId, anim.animName, anim.loop);
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