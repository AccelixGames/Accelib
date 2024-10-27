using System;
using Accelib.Data;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Tween.Effect.Base
{
    public abstract class SimpleEffectTween : MonoBehaviour
    {
        [field: Header("Base Option")]
        [field: SerializeField] public InitModeType InitMode { get; private set; } = InitModeType.None;
        [field: SerializeField] public DisableModeType DisableMode { get; private set; } = DisableModeType.None;
        [field: SerializeField] public EasePairTweenConfig Config { get; private set; }
        
        [field: Header("Sequence Option")]
        
        [field: SerializeField] public SequenceModeType SequenceMode { get; private set; } = SequenceModeType.None;
        [field: SerializeField] public int Priority { get; private set; } = 0;

        private Tweener _tween;

        protected virtual void OnEnable()
        {
            if (InitMode == InitModeType.EnableEffect)
                EnableEffect();
            else if (InitMode == InitModeType.DisableEffect)
                DisableEffect();
        }

        protected virtual void OnDisable() => _tween?.Kill();

        public Tweener EnableEffect(bool resetOnStart = true)
        {
            _tween?.Kill();
            _tween = Internal_EnableEffect(resetOnStart);
            _tween.SetDelay(Config.delayA);
            _tween.SetEase(Config.easeA);
            return _tween;
        }
        
        public Tweener DisableEffect(bool resetOnStart = true)
        {
            _tween?.Kill();
            _tween = Internal_DisableEffect(resetOnStart);
            _tween.SetDelay(Config.delayB);
            _tween.SetEase(Config.easeB);
            
            if(DisableMode == DisableModeType.Disable)
                _tween.onComplete += () => gameObject.SetActive(false);
            else if(DisableMode == DisableModeType.Destroy)
                _tween.onComplete += () => Destroy(gameObject);

            return _tween;
        }

        protected abstract Tweener Internal_EnableEffect(bool resetOnStart = true);
        protected abstract Tweener Internal_DisableEffect(bool resetOnStart = true);
        
#if UNITY_EDITOR
        [Button(enabledMode: EButtonEnableMode.Playmode)] private void PlayEnableEffect() => EnableEffect();
        [Button(enabledMode: EButtonEnableMode.Playmode)] private void PlayDisableEffect() => DisableEffect();
#endif
    }
}