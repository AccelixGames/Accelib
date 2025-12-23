using Accelib.Module.Localization.Architecture;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.Localization.Helper
{
    public class LocalizedEvent : MonoBehaviour, ILocaleChangedEventListener
    {
        [SerializeField] private string key;
        [SerializeField] private bool loadOnEnable = true;
        
        [Header("Events")]
        [SerializeField] private UnityEvent<string> onLocaleChanged = new();
        
        public string LocaleKey => key;
        public int FontIndex => 0;
        public bool IsEnabled => enabled;
        public bool LoadOnEnable => loadOnEnable;
        
        private void OnEnable()
        {
            if (loadOnEnable) 
                Reload();
        }

        [Button("다시 로드", EButtonEnableMode.Playmode)]
        public string Reload()
        {
            // 현지화된 텍스트 가져오기
            var localizedString = LocalizationSingleton.GetLocalizedString(LocaleKey, this);
            
            // 업데이트 이벤트 호출
            OnLocaleUpdated(localizedString);
            
            // 반환
            return localizedString;
        }
        
        public void OnLocaleUpdated(string localizedString)
        {
            onLocaleChanged.Invoke(localizedString);
        }
    }
}