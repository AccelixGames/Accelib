using NaughtyAttributes;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Accelib.Extension.Atom.UI
{
    public class SimpleHpBar : MonoBehaviour
    {
        [Header("Var")]
        [SerializeField] private IntVariable currVar;
        [SerializeField] private IntVariable maxVar;

        [Header("UI")]
        [SerializeField] private TMP_Text currTMP;
        [SerializeField] private TMP_Text maxTMP;
        [SerializeField] private Slider slider;

        [Header("Values")]
        [SerializeField, ReadOnly] private float prevNormal;
        [SerializeField, ReadOnly] private float normal;
        [SerializeField] private UnityEvent<float> onNormalChanged;
        [SerializeField] private UnityEvent<float> onNormalChangedPositive;
        [SerializeField] private UnityEvent<float> onNormalChangedNegative;

        private void OnEnable()
        {
            slider.wholeNumbers = false;

            prevNormal = 0f;
            normal = 0f;
            
            currVar.Changed.Register(OnChanged);
            maxVar.Changed.Register(OnChanged);
            OnChanged();
        }

        private void OnDisable()
        {
            currVar.Changed.Unregister(OnChanged);
            maxVar.Changed.Unregister(OnChanged);
        }

        private void OnChanged()
        {
            var val = currVar?.Value ?? 0;
            var max = maxVar?.Value ?? 0;
            prevNormal = normal;
            normal = max == 0 ? 0f : val / (float)max;
            
            if(currTMP) currTMP.text = val.ToString();
            if(maxTMP) maxTMP.text = max.ToString();
            if(slider) slider.value = normal;
            
            if(Mathf.Approximately(prevNormal, normal)) return;
            
            onNormalChanged?.Invoke(normal);
            if(prevNormal < normal) onNormalChangedPositive?.Invoke(normal);
            if(prevNormal > normal) onNormalChangedNegative?.Invoke(normal);
        }
    }
}