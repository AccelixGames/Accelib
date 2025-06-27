using Accelib.Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable InconsistentNaming

namespace Accelib.Extensions
{
    public static class TweenExtension
    {
        public static Tweener DOShakeScale(this Transform transform, ShakeTweenConfig config) =>
            transform.DOShakeScale(config.duration, config.strength, config.vibrato, config.randomness,
                config.fadeOut, config.randomnessMode).SetDelay(config.delay);

        public static Tweener DOShakePosition(this Transform transform, ShakeTweenConfig config) =>
            transform.DOShakePosition(config.duration, config.strength, config.vibrato, config.randomness,
                config.snapping, config.fadeOut, config.randomnessMode).SetDelay(config.delay);
        
        public static Tweener DOShakeAnchorPos(this RectTransform transform, ShakeTweenConfig config) =>
            transform.DOShakeAnchorPos(config.duration, config.strength, config.vibrato, config.randomness,
                config.snapping, config.fadeOut, config.randomnessMode).SetDelay(config.delay);

        public static Sequence DOPingPongRotation(this Transform transform, Vector3 initValue, Vector3 endValue, float duration, int count)
        {
            var seq = DOTween.Sequence();
            var unitDuration = duration / count;
            var halfDuration = unitDuration * 0.5f;

            seq.Append(transform.DORotate(endValue, halfDuration));
            for (var i = 0; i < count; i++)
            {
                var value = i % 2 == 0 ? -endValue : endValue;
                seq.Append(transform.DORotate(value, unitDuration));
            }
            seq.Append(transform.DORotate(initValue, halfDuration));

            return seq;
        }
    }
}