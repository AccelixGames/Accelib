using System;
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
        [SerializeField] private TransitionEffect[] targetEffects;
        [SerializeField, ReadOnly] private bool isMoving;
        [SerializeField, ReadOnly] private int currIndex;

        private Sequence _seq;

        private void Start()
        {
            currIndex = 0;
        }

        public static Sequence StartTransition(int index = 0)
        {
            if (!Instance) 
                return null;

            Instance.currIndex = index;
            return Instance.Transition(true);
        }

        public static Sequence EndTransition()
        {
            if (!Instance) 
                return null;
            
            return Instance.Transition(false);
        }

        private Sequence Transition(bool start)
        {
            _seq?.Kill();
            _seq = start ? targetEffects[currIndex].StartTransition() : targetEffects[currIndex].EndTransition();
            
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