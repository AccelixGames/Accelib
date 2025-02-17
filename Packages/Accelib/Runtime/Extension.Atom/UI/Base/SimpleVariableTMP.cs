using TMPro;
using UnityAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Extension.Atom.UI.Base
{
    public abstract class SimpleVariableTMP<T, T2> : MonoBehaviour where T: AtomBaseVariable
    {
        [Header("Core")]
        [SerializeField] protected TMP_Text tmp;
        [SerializeField] protected T variable;

        [Header("Modifier")]
        [SerializeField] protected string format;
#if UNITY_EDITOR
        [SerializeField, ReadOnly] protected string preview;
#endif

        [Header("Event")]
        [SerializeField] private UnityEvent<T2> onUpdate;

        protected abstract AtomEvent<T2> Changed { get; } 
        protected abstract T2 GetValue { get; }
        
        private void OnEnable()
        {
            Changed?.Register(OnChanged);
            OnChanged(GetValue);
        }

        private void OnDisable() => Changed?.Unregister(OnChanged);

        private void OnChanged(T2 v)
        {
            if (!tmp) return;
            
            tmp.text = GetText(v);
            onUpdate?.Invoke(v);
        }

        protected virtual string GetText(T2 v)
        {
            if (string.IsNullOrEmpty(format))
                return v.ToString();
            
            return string.Format(format, v);
        }
        
#if UNITY_EDITOR
        private void Reset() => tmp = GetComponent<TMP_Text>();
        protected virtual void OnValidate() => preview = GetText(GetValue);
#endif
    }
}