using Accelib.Data;
using DG.Tweening;
using Sirenix.OdinInspector;
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

        [Button, EnableIf("@UnityEngine.Application.isPlaying")]
        public void Shake() => DoShake();
        
        [Button, EnableIf("@UnityEngine.Application.isPlaying")]
        public void ShakeRotation() => DoShakeRotation();
    }
}