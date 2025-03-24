using System;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Extension.Atom.UI
{
    public class SimpleScoreTMP : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private TMP_Text tmp;
        [SerializeField] private IntVariable variable;
        
        [Header("Value")]
        [SerializeField, Range(0f, 10f)] private float duration = 0.5f;
        [SerializeField, ReadOnly] private int currValue; 
        [SerializeField, ReadOnly] private int targetValue;

        [Header("Event")]
        public UnityEvent onChanged;

        private Tweener _tween;
        
        private void Start()
        {
            currValue = 0;
            targetValue = 0;
        }

        private void OnEnable()
        {
            variable.Changed?.Register(OnChanged);
            OnChanged(variable.Value);
        }
        
        private void OnDisable() => variable.Changed?.Unregister(OnChanged);

        private void OnChanged(int value)
        {
            if (targetValue == value) return;
            
            targetValue = value;
            
            _tween?.Kill();
            _tween = DOTween.To(() => currValue, x => currValue = x, targetValue, duration)
                .OnUpdate(() => tmp.text = currValue.ToString());
            
            onChanged?.Invoke();
        }
    }
}