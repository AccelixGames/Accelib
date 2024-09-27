using Accelib.Data;
using DG.Tweening;
using UnityEngine;

namespace Accelib.Extensions
{
    public static class TweenExtension
    {
        public static Tweener DOShakePosition(this Transform transform, ShakeTweenConfig config) =>
            transform.DOShakePosition(config.duration, config.strength, config.vibrato, config.randomness,
                config.snapping, config.fadeOut, config.randomnessMode).SetDelay(config.delay);
    }
}