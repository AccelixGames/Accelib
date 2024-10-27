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

        private Sequence _seq;
        
        private void OnEnable()
        {
            if (startMode == FadeMode.In)
                EffectIn();
            else if (startMode == FadeMode.Out)
                EffectOut();
        }

        private void OnDisable() => _seq?.Kill();

        public Sequence EffectIn()
        {
            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject);
            _seq.AppendCallback(() =>
            {
                gameObject.SetActive(true);
                transform.localScale = disabledScale;
            });
            _seq.Append(transform.DOScale(Vector3.one, config.duration)
                .SetEase(config.easeA)
                .SetDelay(config.delayA));

            return _seq;
        }

        public Sequence EffectOut()
        {
            if (!gameObject.activeSelf) return null;

            _seq?.Kill();
            _seq = DOTween.Sequence().SetLink(gameObject);
            _seq.Append(transform.DOScale(disabledScale, config.duration)
                .SetEase(config.easeB)
                .SetDelay(config.delayB));
            _seq.OnComplete(() => gameObject.SetActive(false));

            return _seq;
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void OnSetDisable() => EffectOut();
    }
}