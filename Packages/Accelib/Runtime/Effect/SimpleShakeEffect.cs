using Accelib.Data;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimpleShakeEffect : MonoBehaviour
    {
        [Header("타겟")] 
        [SerializeField] private Transform target;
        [SerializeField] private ShakeTweenConfig tweenConfig;

        public DG.Tweening.Tween DoShake() => 
            target.DOShakePosition(tweenConfig.duration, tweenConfig.strength, tweenConfig.vibrato, tweenConfig.randomness, 
                tweenConfig.snapping, tweenConfig.fadeOut, tweenConfig.randomnessMode);
        
        public DG.Tweening.Tween DoShakeRotation() => 
            target.DOShakeRotation(tweenConfig.duration, tweenConfig.strength, tweenConfig.vibrato, tweenConfig.randomness, 
                tweenConfig.fadeOut, tweenConfig.randomnessMode);

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void Shake() => DoShake();
        
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void ShakeRotation() => DoShakeRotation();
    }
}