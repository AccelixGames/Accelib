using System;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Accelib.Module.UI.Utility
{
    public class SliderEventExtender : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        [Header("Events")]
        [SerializeField] private bool useFloat = false;
        [SerializeField, ShowIf(nameof(useFloat))] private UnityEvent<float> onChangedFloat;
        
        [SerializeField] private bool useInt = false;
        [SerializeField, ShowIf(nameof(useInt))] private UnityEvent<int> onChangedInt;
        
        [SerializeField] private bool useNormal = false;
        [SerializeField, ShowIf(nameof(useNormal))] private UnityEvent<float> onChangedNormal;
        
        private void OnEnable() => slider.onValueChanged.AddListener(OnChanged);
        private void OnDisable() => slider.onValueChanged.RemoveListener(OnChanged);

        private void OnChanged(float value)
        {
            if(useFloat) onChangedFloat?.Invoke(value);
            if(useInt) onChangedInt?.Invoke((int)value);
            if(useNormal) onChangedNormal?.Invoke(value - slider.minValue / slider.maxValue - slider.minValue);
        }
    }
}