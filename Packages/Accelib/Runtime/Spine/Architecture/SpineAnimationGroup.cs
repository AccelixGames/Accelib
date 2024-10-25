#if ACCELIX_SPINE
using NaughtyAttributes;
using Spine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

namespace Accelib.Spine.Architecture
{
    [System.Serializable]
    public class SpineAnimationGroup
    {
        public SpineAnimMethod method = SpineAnimMethod.Set;
        [Range(0, 5)] public int trackId = 0;
        [SpineAnimation(dataField = "skeletonDataAsset")] public string animName;
        public bool loop = false;
        [ShowIf(nameof(method), SpineAnimMethod.Add), Range(0f, 5f)]
        public float delay = 0f;

        
        public TrackEntry Play(AnimationState state) =>
            method == SpineAnimMethod.Add
                ? state.AddAnimation(trackId, animName, loop, delay)
                : state.SetAnimation(trackId, animName, loop);
    }
}
#endif