using Spine;

namespace Accelib.Spine.Extensions
{
    public static class SkeletonAnimationExtension
    {
        public static TrackEntry SetAnimationWithoutDuplication(this AnimationState animState, int trackIndex, string animation, bool loop)
        {
            var curr = animState.GetCurrent(trackIndex); 
            
            if(curr?.Animation.Name != animation)
                return animState.SetAnimation(trackIndex, animation, loop);

            return curr;
        }
    }
}