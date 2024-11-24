using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.UI.Toggle
{
    public sealed class ToggleBtnGroup : MonoBehaviour
    {
        [Header("옵션")]
        [SerializeField] private bool initOnStart = true;
        [ShowIf(nameof(initOnStart)), SerializeField] private int initialIndex = 0;
        public UnityEvent<int> onToggle;
        public UnityEvent<int> onToggleId;
        public UnityEvent<ToggleBtn> onToggleObject;

        [field: Header("토글 버튼들")]
        [field:SerializeField, ReadOnly] public int CurrIndex { get; private set; }

        [SerializeField, ReadOnly] private ToggleBtn currToggle = null;
        [SerializeField, ReadOnly] private ToggleBtn[] toggles = null;
        
        private void Awake()
        {
            toggles = GetComponentsInChildren<ToggleBtn>();
        }
        
        private void Start()
        {
            if (initOnStart)
            {
                Initialize(initialIndex);
                onToggle?.Invoke(CurrIndex);   
            }
        }

        public void Refresh()
        {
            toggles = GetComponentsInChildren<ToggleBtn>();
            Initialize(CurrIndex);
        }

        public void Initialize(int index)
        {
            // 토글이 감지된게 없다면, 종료
            if (toggles is not { Length: > 0 })
            {
                toggles ??= GetComponentsInChildren<ToggleBtn>();
                if(toggles.Length <= 0)
                    return;
            }
            
            // 인덱스가 토글 개수를 초과한다면,
            if(index >= toggles.Length || index < 0)
                // 0으로 돌리기
                index = 0;
            
            // 현재 인덱스 초기화
            CurrIndex = index;
            
            // 토글 초기화
            for (var i = 0; i < toggles.Length; i++)
                toggles[i].Initialize(this, i == CurrIndex);

            // 현재 토글 설정 
            currToggle = toggles[CurrIndex];
        }

        public void ToggleLeft() => ToggleIndex(-1);
        public void ToggleRight() => ToggleIndex(1);

        private void ToggleIndex(int index)
        {
            var count = toggles.Length;
            var leftIndex = (CurrIndex + count + index) % count;
            var toggle = toggles[leftIndex];
            Toggle(toggle);
        }
        
        internal void Toggle(ToggleBtn target)
        {
            if (toggles is not { Length: > 0 }) return;
            if (target == null) return;
            if (currToggle == target) return;

            for (var i = 0; i < toggles.Length; i++)
            {
                var toggle = toggles[i];
                if (toggle != target) continue;

                currToggle.Toggle(false);
                currToggle = toggle;
                currToggle.Toggle(true);
                CurrIndex = i;
                onToggle?.Invoke(i);
                onToggleId?.Invoke(currToggle.Id);
                onToggleObject?.Invoke(currToggle);
                return;
            }
        }
    }
}