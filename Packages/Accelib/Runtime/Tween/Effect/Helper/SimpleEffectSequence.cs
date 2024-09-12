using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Tween.Effect.Base;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Tween.Effect.Helper
{
    public sealed class SimpleEffectSequence : MonoBehaviour
    {
        [field: SerializeField] public InitModeType InitMode { get; private set; } = InitModeType.None;
        [field: SerializeField] public DisableModeType DisableMode { get; private set; } = DisableModeType.None;
        [SerializeField, ReadOnly] private List<SimpleEffectTween> tweenList;

        private Sequence _sequence;
        
        private void Awake()
        {
            GetComponents(tweenList);
            tweenList?.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        private void OnEnable()
        {
            if (InitMode == InitModeType.EnableEffect)
                EnableEffect();
            else if (InitMode == InitModeType.DisableEffect)
                DisableEffect();
        }

        private void OnDisable() => _sequence?.Kill();

        public Sequence EnableEffect(bool resetOnStart = true)
        {
            if (tweenList is not { Count: > 0 }) return null;
            
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            foreach (var tween in tweenList)
            {
                if (tween.SequenceMode == SequenceModeType.Append)
                    _sequence.Append(tween.EnableEffect(resetOnStart));
                else if (tween.SequenceMode == SequenceModeType.Join)
                    _sequence.Join(tween.EnableEffect(resetOnStart));
            }
            
            if(DisableMode == DisableModeType.Disable)
                _sequence.onComplete += () => gameObject.SetActive(false);
            else if(DisableMode == DisableModeType.Destroy)
                _sequence.onComplete += () => Destroy(gameObject);

            return _sequence;
        }
        
        public Sequence DisableEffect(bool resetOnStart = true)
        {
            if (tweenList is not { Count: > 0 }) return null;
            
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            
            foreach (var tween in tweenList)
            {
                if (tween.SequenceMode == SequenceModeType.Append)
                    _sequence.Append(tween.DisableEffect(resetOnStart));
                else if (tween.SequenceMode == SequenceModeType.Join)
                    _sequence.Join(tween.DisableEffect(resetOnStart));
                else
                    continue;
            }

            return _sequence;
        }
        
#if UNITY_EDITOR
        [Button(enabledMode: EButtonEnableMode.Playmode)] private void PlayEnableEffect() => EnableEffect();
        [Button(enabledMode: EButtonEnableMode.Playmode)] private void PlayDisableEffect() => DisableEffect();
#endif
    }
}