#if ACCELIX_SPINE
using System;
using Accelib.Logging;
using Spine;
using Spine.Unity;
using Object = UnityEngine.Object;

namespace Accelib.Spine.Extensions
{
    public static class SkeletonAnimationExtension
    {
        public static void UpdateSkin(this Skeleton skeleton, string skinName, Object obj = null)
        {
            try
            {
                skeleton.SetSkin(skinName);
                skeleton.SetSlotsToSetupPose();
            }
            catch (Exception e)
            {
                Deb.LogWarning(e.Message, obj);
            }
        }
        
        public static void UpdateSkin(this SkeletonGraphic skeleton, string skinName) => 
            skeleton.Skeleton.UpdateSkin(skinName, skeleton);

        public static void UpdateSkin(this SkeletonAnimation skeleton, string skinName)=>
            skeleton.Skeleton.UpdateSkin(skinName, skeleton);

        
        public static TrackEntry SetAnimationWithoutDuplication(this AnimationState animState, int trackIndex, string animation, bool loop, float? mixDuration =null, float? delay = null)
        {
            var curr = animState.GetCurrent(trackIndex);

            if (curr?.Animation.Name != animation)
            {
                curr = animState.SetAnimation(trackIndex, animation, loop);

                if (mixDuration.HasValue || delay.HasValue)
                {
                    curr.MixDuration = mixDuration.GetValueOrDefault(0f);
                    curr.Delay = delay.GetValueOrDefault(0f);
                }
            }

            return curr;
        }
        
        public static TrackEntry AddAnimationWithoutDuplication(this AnimationState animState, int trackIndex, string animation, bool loop, float delay)
        {
            var curr = animState.GetCurrent(trackIndex);

            if (curr?.Animation.Name != animation)
                return animState.AddAnimation(trackIndex, animation, loop, delay);

            return curr;
        }
    }
}
#endif