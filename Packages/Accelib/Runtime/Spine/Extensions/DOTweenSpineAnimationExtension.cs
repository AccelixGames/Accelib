#if ACCELIX_SPINE
using DG.Tweening;
using Spine;
using Spine.Unity;

namespace Accelib.Spine.Extensions
{
    public static class DOTweenSpineAnimationExtension
    {
        public static Tweener DOTimeScale(this AnimationState skeleton, float endValue, float duration)
            => DOTween.To(() => skeleton.TimeScale, t => skeleton.TimeScale = t, endValue, duration);
        
        public static Tweener DOTimeScale(this TrackEntry track, float endValue, float duration)
            => DOTween.To(() => track.TimeScale, t => track.TimeScale = t, endValue, duration);

        public static Tweener DOAlpha(this TrackEntry track, float endValue, float duration)
            => DOTween.To(() => track.Alpha, t => track.Alpha = t, endValue, duration);

        public static Tweener DOTrackTime(this TrackEntry track, float endValue, float duration)
            => DOTween.To(() => track.TrackTime, t => track.TrackTime = t, endValue, duration);

        public static Tweener DOOverrideAlpha(this SkeletonUtilityBone bone, float endValue, float duration)
            => DOTween.To(() => bone.overrideAlpha, x => bone.overrideAlpha = x, endValue, duration);
    }
}
#endif