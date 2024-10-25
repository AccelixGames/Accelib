#if ACCELIX_SPINE
using DG.Tweening;
using NaughtyAttributes;
using Spine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

namespace Accelib.Spine.Architecture
{
    [System.Serializable]
    public class SpineAnimationRefGroup
    {
        [AllowNesting]
        public SpineAnimMethod method = SpineAnimMethod.Set;

        [AllowNesting] [Range(0, 5)] public int trackId = 0;

        [AllowNesting] [HideIf(nameof(method), SpineAnimMethod.Clear)]
        public AnimationReferenceAsset animRef;

        [AllowNesting] [HideIf(nameof(method), SpineAnimMethod.Clear)]
        public bool loop = false;

        [AllowNesting] [ShowIf(nameof(method), SpineAnimMethod.Add), Range(0f, 5f)]
        public float delay = 0f;

        public TrackEntry SetOrAdd(AnimationState state) =>
            method == SpineAnimMethod.Add
                ? state.AddAnimation(trackId, animRef, loop, delay)
                : state.SetAnimation(trackId, animRef, loop);
    }
}
#endif