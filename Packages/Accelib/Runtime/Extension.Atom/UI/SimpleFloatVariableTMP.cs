using System;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Extension.Atom.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class SimpleFloatVariableTMP : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmp;
        [SerializeField] private FloatVariable targetVar;

        [Header("Modifier")]
        [SerializeField] private string format = "F1";

        private void OnEnable()
        {
            targetVar.Changed.Register(OnChanged);
            OnChanged(targetVar.Value);
        }

        private void OnDisable() => targetVar.Changed.Unregister(OnChanged);

        private void OnChanged(float v)
        {
            if (string.IsNullOrEmpty(format))
                tmp.text = v.ToString();
            else
                tmp.text = v.ToString(format);
        }

        private void Reset() => tmp = GetComponent<TMP_Text>();
    }
}