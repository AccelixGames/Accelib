using System;
using Accelib.Data;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimpleSpriteColorPingPongEffect: MonoBehaviour
    {
        [SerializeField] private EasePairTweenConfig config;
        [SerializeField] private SpriteRenderer render;
        
        [Header("설정")] 
        [SerializeField] private bool playOnStart = false;
        [SerializeField] private int loopCount = -1;
        [SerializeField] private Color colorInit = Color.white;
        [SerializeField] private Color colorTo = Color.white;
        
        private Sequence _seq;

        private void Start()
        {
            _seq = DOTween.Sequence()
                .Append(render.DOColor(colorInit, config.durationB).SetEase(config.easeB))
                .Append(render.DOColor(colorTo, config.durationA).SetEase(config.easeA))
                .SetUpdate(true).SetAutoKill(false).SetLink(gameObject).Pause();
        }

        private void OnEnable()
        {
            if (playOnStart)
                Blink();
        }

        private void OnDisable() => _seq?.Kill();

        [Button]
        public Sequence Blink()
        {
            _seq?.Play().Restart();
            return _seq;
        }
    }
}