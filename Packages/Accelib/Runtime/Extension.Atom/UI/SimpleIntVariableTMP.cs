using System;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Extension.Atom.UI
{
    //[RequireComponent(typeof(TMP_Text))]
    public class SimpleIntVariableTMP : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmp;
        [SerializeField] private IntVariable intVar;

        [Header("Modifier")]
        [SerializeField] private string format;

        [Header("Event")]
        [SerializeField] private UnityEvent<int> onUpdate;

        private void OnEnable()
        {
            intVar.Changed.Register(OnChanged);
            OnChanged(intVar.Value);
        }

        private void OnDisable() => intVar.Changed.Unregister(OnChanged);

        private void OnChanged(int v)
        {
            if (string.IsNullOrEmpty(format))
                tmp.text = v.ToString();
            else
                tmp.text = v.ToString(format);
            
            onUpdate?.Invoke(v);
        }

        private void Reset() => tmp = GetComponent<TMP_Text>();
    }
}