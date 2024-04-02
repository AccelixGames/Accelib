using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimpleShakeEffect : MonoBehaviour
    {
        [Header("타겟")] 
        [SerializeField] private Transform target;
        
        [Header("수치")]
        [SerializeField] private float duration = 1f;
        [SerializeField] private float strength = 1f;
        [SerializeField] private int vibrato = 30;
        [SerializeField] private float randomness = 90f;
        [SerializeField] private bool snapping = false;
        [SerializeField] private bool fadeOut = true;
        [SerializeField] private ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full;

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public DG.Tweening.Tween DoShake() => 
            target.DOShakePosition(duration, strength, vibrato, randomness, snapping, fadeOut, randomnessMode);

        private void Reset()
        {
            duration = 1f;
            strength = 1f;
            vibrato = 30;
            randomness = 90f;
            snapping = false;
            fadeOut = true;
            randomnessMode = ShakeRandomnessMode.Full;
        }
    }
}