using Spine;
using Spine.Unity;

namespace Accelib.Spine.Extensions
{
    public static class SkeletonAnimationExtension
    {
        public static void UpdateSkin(this SkeletonAnimation skeleton, string skinName)
        {
            skeleton.Skeleton.SetSkin(skinName);
            skeleton.Skeleton.SetSlotsToSetupPose();
        }

        
        public static TrackEntry SetAnimationWithoutDuplication(this AnimationState animState, int trackIndex, string animation, bool loop, float? mixDuration =null, float? delay = null)
        {
            var curr = animState.GetCurrent(trackIndex);

            if (curr?.Animation.Name != animation)
            {
                curr = animState.SetAnimation(trackIndex, animation, loop);

                if (mixDuration.HasValue || delay.HasValue)
                    curr.SetMixDuration(mixDuration.GetValueOrDefault(0f), delay.GetValueOrDefault(0f));
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