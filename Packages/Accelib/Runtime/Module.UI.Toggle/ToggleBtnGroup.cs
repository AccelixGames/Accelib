using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.UI.Toggle
{
    public sealed class ToggleBtnGroup : MonoBehaviour
    {
        [Header("옵션")]
        [SerializeField] private int initialIndex = 0;
        public UnityEvent<int> onToggle;
        
        [Header("토글 버튼들")]
        [SerializeField, ReadOnly] private ToggleBtn[] toggles = null;
        [SerializeField, ReadOnly] private ToggleBtn currToggle = null;
        
        private void Awake()
        {
            toggles = GetComponentsInChildren<ToggleBtn>();
        }

        private void Start()
        {
            if (toggles is not { Length: > 0 }) return;

            if(initialIndex >= toggles.Length || initialIndex < 0) 
                initialIndex = 0;
            
            for (var i = 0; i < toggles.Length; i++)
                toggles[i].Initialize(this, i == initialIndex);

            currToggle = toggles[initialIndex];
            onToggle?.Invoke(initialIndex);
        }

        internal void Toggle(ToggleBtn target)
        {
            if (toggles is not { Length: > 0 }) return;
            if (target == null) return;

            for (var i = 0; i < toggles.Length; i++)
            {
                var toggle = toggles[i];
                if (toggle != target) continue;

                currToggle.Toggle(false);
                currToggle = toggle;
                currToggle.Toggle(true);
                onToggle?.Invoke(i);
                return;
            }
        }
    }
}