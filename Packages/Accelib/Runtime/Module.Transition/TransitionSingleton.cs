using System;
using System.Linq;
using Accelib.Core;
using Accelib.Module.Audio;
using Accelib.Module.Audio.Data;
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

        [Header("오디오")]
        [SerializeField, Range(0f, 1f)] private float transitionControlVolume = 1f;

        private Sequence _seq;
        public static bool IsActive => Instance?.targetEffects.Any(x => x.IsActive) ?? false;

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
            var targetVolume = start ? transitionControlVolume : 1f;

            _seq.AppendCallback(() => AudioSingleton.SetControlVolume(AudioChannel.Bgm, targetVolume));
            
            return _seq;
        }

#if UNITY_EDITOR
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void TrStart() => Transition(true);

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void TrEnd() => Transition(false);
#endif
    }
}