﻿using System;
using Accelib.Data;
using DG.Tweening;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimpleBlinkEffect : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private EasePairTweenConfig config;

        [Header("설정")] 
        [SerializeField] private bool playOnStart = false;
        [SerializeField] private int loopCount = -1;
        [SerializeField, Range(0f, 1f)] private float maxAlpha = 1f;
        [SerializeField, Range(0f, 1f)] private float minAlpha = 0f;

        private Sequence seq;

        private void OnEnable()
        {
            if (playOnStart)
                Blink();
        }

        private void OnDisable() => seq?.Kill();

        public DG.Tweening.Tween Blink()
        {
            gameObject.SetActive(true);
            //group.alpha = 0f;
            seq?.Kill();
            
            var duration = config.duration * 0.5f;
            seq = DOTween.Sequence()
                .Append(group.DOFade(minAlpha, duration).SetEase(config.easeA))
                .Append(group.DOFade(maxAlpha, duration).SetEase(config.easeB))
                .SetLoops(loopCount);

            return seq;
        }
    }
}