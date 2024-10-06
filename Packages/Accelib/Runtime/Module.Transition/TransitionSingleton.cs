using Accelib.Core;
using Accelib.Module.Transition.Effect;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.Transition
{
    public class TransitionSingleton : MonoSingleton<TransitionSingleton>
    {
        [Header("상태")]
        [SerializeField] private TransitionEffect targetEffect;
        [SerializeField, ReadOnly] private bool isMoving;

        private Sequence _seq;
        
        public static Sequence StartTransition()
        {
            if (Instance == null) return null;

            return Instance.Transition(true);
        }

        public static Sequence EndTransition()
        {
            if (Instance == null) return null;
            
            return Instance.Transition(false);
        }

        private Sequence Transition(bool start)
        {
            _seq?.Kill();
            _seq = start ? targetEffect.StartTransition() : targetEffect.EndTransition();
            
            return _seq;
        }

#if UNITY_EDITOR
        [Button]
        public void TrStart() => Transition(true);

        [Button]
        public void TrEnd() => Transition(false);
#endif
    }
}