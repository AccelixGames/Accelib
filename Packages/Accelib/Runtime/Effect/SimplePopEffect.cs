using System;
using Accelib.Data;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimplePopEffect : MonoBehaviour
    {
        private enum FadeMode { In, Out, None }
        
        [SerializeField] private EasePairTweenConfig config;
        
        [Header("설정")] 
        [SerializeField] private FadeMode startMode = FadeMode.In;
        [SerializeField] private Vector3 disabledScale = Vector3.one * 0.0001f;
        [SerializeField] private bool ignoreTimeScale = false;

        private Sequence _seq;
        
        private void OnEnable()
        {
            if (startMode == FadeMode.In)
                DoEffectIn();
            else if (startMode == FadeMode.Out)
                DoEffectOut();
        }

        private void OnDisable() => _seq?.Kill();

        public Sequence DoEffectIn()
        {
            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject)
                .SetUpdate(ignoreTimeScale);
            _seq.AppendCallback(() =>
            {
                gameObject.SetActive(true);
                transform.localScale = disabledScale;
            });
            _seq.Append(transform.DOScale(Vector3.one, config.duration)
                .SetEase(config.easeA, config.overshoot)
                .SetDelay(config.delayA));

            return _seq;
        }
        public void EffectIn() => DoEffectIn();

        public Sequence DoEffectOut()
        {
            if (!gameObject.activeSelf) return null;

            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject)
                .SetUpdate(ignoreTimeScale);
            _seq.Append(transform.DOScale(disabledScale, config.duration)
                .SetEase(config.easeB ,config.overshoot)
                .SetDelay(config.delayB));
            _seq.OnComplete(() => gameObject.SetActive(false));

            return _seq;
        }
        public void EffectOut() => DoEffectOut();

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void OnSetDisable() => DoEffectOut();
    }
}