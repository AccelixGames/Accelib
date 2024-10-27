using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Accelib.Data
{
    [CreateAssetMenu(fileName = "tween-", menuName = "Accelix/Tween/EasePair", order = 0)]
    public class EasePairTweenConfig : ScriptableObject
    {
        [FormerlySerializedAs("delay")]
        [Range(0f, 10f)] public float delayA = 0f;
        [FormerlySerializedAs("delay")]
        [Range(0f, 10f)] public float delayB = 0f;
        [Range(0.01f, 10f)] public float duration = 1f;
        public Ease easeA = Ease.OutBack;
        public Ease easeB = Ease.InBack;
        
        [Header("Additional Option")]
        public float overshoot = 1.70158f;

        public Ease GetEase(bool isIn) => isIn ? easeA : easeB;
    }

    public static class EasePairTweenConfigExtension
    {
        public static Tweener SetEase(this Tweener tween, EasePairTweenConfig config, bool isOn = true)
        {
            var ease = config.GetEase(isOn);

            if (ease is Ease.InElastic or Ease.OutElastic or Ease.InOutElastic)
                return tween.SetEase(ease, config.overshoot);
            else
                return tween.SetEase(ease);
        }
    }
}