using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimplePunchEffect : MonoBehaviour
    {
        [SerializeField] private Vector3 punch = Vector3.one;
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private int vibrato = 10;
        [SerializeField] private float elasticity = 1f;

        private Tweener _tween;
        
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void DoPunch()
        {
            _tween?.Kill(true);
            _tween = transform.DOPunchScale(punch, duration, vibrato, elasticity);
        }
    }
}