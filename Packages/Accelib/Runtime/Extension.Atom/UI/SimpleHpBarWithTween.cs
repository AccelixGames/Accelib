using System;
using Accelib.Data;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.Extension.Atom.UI
{
    public class SimpleHpBarWithTween : MonoBehaviour
    {
        [Header("# UI")]
        [SerializeField] private TMP_Text currTMP;
        [SerializeField] private TMP_Text maxTMP;
        [SerializeField] private Slider slider;
        
        [Header("# 데이터")]
        [SerializeField, ReadOnly] private int currValue = 0;
        [SerializeField, ReadOnly] private int goalValue = 0;
        [SerializeField, ReadOnly] private int maxValue = 0;
        [SerializeField, ReadOnly, Range(0f, 1f)] private float normal = 0f;

        [Header("# Ease")]
        [SerializeField] private DefaultTweenConfig easeConfig;
        
        private Tweener _tween;

        private void Start()
        {
            _tween = DOTween.To(() => currValue, x => currValue = x, goalValue, easeConfig.duration)
                .SetEase(easeConfig.ease)
                .OnUpdate(OnUpdate)
                .SetLink(gameObject)
                .SetAutoKill(false)
                .Pause();

            UpdateMaxValue(maxValue);
        }
        private void OnDisable() => _tween?.Pause();
        private void OnDestroy() => _tween?.Kill();
        private void OnUpdate()
        {
            normal = maxValue <= 0 ? 0f : Mathf.Clamp01(currValue / (float)maxValue);
            
            currTMP.text = currValue.ToString();
            slider.normalizedValue = normal;
        }

        public void UpdateCurrValue(int value)
        {
            goalValue = value;
            if (_tween == null)
            {
                currValue = goalValue;
                OnUpdate();
            }
            else
            {
                _tween.ChangeEndValue(goalValue, true).Restart();
            }
        }

        public void UpdateMaxValue(int value)
        {
            maxValue = value;
            maxTMP.text = value.ToString();
            
            OnUpdate();
        }
    }
}