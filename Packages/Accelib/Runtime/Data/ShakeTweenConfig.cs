using DG.Tweening;
using UnityEngine;

namespace Accelib.Data
{
    [CreateAssetMenu(fileName = "tween-", menuName = "Accelix/Tween/Shake", order = 0)]
    public class ShakeTweenConfig : ScriptableObject
    {
        [Header("Base")]
        [Range(0.01f, 10f)] public float duration = 1f;
        public Vector3 strength = Vector3.one;
        public int vibrato = 10;
        public float randomness = 90f;
        public bool snapping = false;
        public bool fadeOut = true;
        public ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full;
        
        [Header("Modifier")]
        [Range(0f, 10f)] public float delay = 0f;
        public Ease ease = Ease.Linear;
    }
}