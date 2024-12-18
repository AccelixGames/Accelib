using System.Collections.Generic;
using System.Linq;
using Accelib.Core;
using Accelib.Logging;
using Accelib.Module.Initialization.Base;
using Accelib.Module.Localization.Architecture;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Module.Localization
{
    public class LocalizationSingleton : MonoSingleton<LocalizationSingleton>, ILateInitRequired
    {
        private const string NullString = "@null@";
        
        // 지원하는 로케일 데이터들
        [SerializeField] private List<LocaleSO> locales;
        [SerializeField] private IntVariable currLangId;
        
        [Header("미리보기")]
        [SerializeField, ReadOnly] private LocaleSO currLocale = null;
        [SerializeField, ReadOnly] private bool isInitialized = false;
        public bool IsInitialized() => isInitialized;

        public SystemLanguage CurrLang => (SystemLanguage)currLangId.Value;
        private bool IsSupportedLang(SystemLanguage lang) => locales.Any(x => x.Language == lang);

        public int Priority => 0;

        public void Init()
        {
            var systemLang = Application.systemLanguage;
            Deb.Log($"시스템 언어: {systemLang}, 저장된 언어: {(SystemLanguage)currLangId.Value}");
            
            // 저장된 언어가 없다면,
            if(!IsSupportedLang(CurrLang))
                // 현재 시스템 언어를 가져옴
                currLangId.SetValue((int)systemLang);

            // 지원하는 언어가 아닐 경우, 
            if (!IsSupportedLang(CurrLang))
                // 첫번째 언어로 설정
                currLangId.Value = (int)locales[0].Language;

            // 로케일 변경
            UpdateLocale();

            isInitialized = true;
        }

        /// <summary>언어를 변경한다.</summary>
        public void ChangeLanguage(SystemLanguage language)
        {
            // 동일한 언어로 변경하려고 할 경우, 종료
            if(CurrLang == language) return;
            
            // 지원하는 언어가 아닐 경우, 종료
            if (locales.All(x => x.Language != language))
            {
                Deb.LogWarning($"{language}는 지원하는 언어가 아닙니다.");
                return;
            }

            // 언어 로드
            currLangId.Value = (int)language;
            
            // 로케일 변경
            UpdateLocale();
        }

        public static string GetLocalizedStringStatic(string key, Object ctx = null) => Instance?.GetLocalizedString(key, ctx) ?? NullString;

        public static LocaleFontData GetFontAssetStatic() => Instance?.currLocale?.FontData;

        /// <summary>키값에 알맞는 로컬라이징 문자열을 가져온다.</summary>
        public string GetLocalizedString(string key, Object ctx = null)
        {
            if(!currLocale || string.IsNullOrEmpty(key))
                return NullString;
            
            // 값 가져오기 실패할 경우,
            if (!currLocale.TextDict.TryGetValue(key, out var result))
            {
                // NULL 리턴
                Deb.LogWarning($"키({key})의 로컬라이징 값을 가져올 수 없습니다. 현재 언어({CurrLang})", ctx);
                result = NullString;
            }
            
            return result;
        }

        // 로케일 값을 업데이트 한다.
        private void UpdateLocale()
        {
            // 로케일 변경
            // Deb.Log($"Update Locale to {CurrLang}({currLangId.Value})");
            currLocale = locales.FirstOrDefault(x => x.Language == CurrLang);
            
            // 변경 이벤트 발생
            foreach (var monoBehaviour in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                // 로케일 변경 이벤트 리스너 가져오기. 없다면 넘기기
                if (!monoBehaviour.TryGetComponent<ILocaleChangedEventListener>(out var listener)) continue;
                // 리스너가 비활 상태라면, 넘기기
                if (!listener.IsEnabled) continue;
                
                // 로케일 문자열 가져와서,
                var localizedString = GetLocalizedString(listener.LocaleKey, monoBehaviour);
                // 업데이트 해주기
                listener.OnLocaleUpdated(localizedString, currLocale?.FontData);
            } 
        }
        
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitSingleton() => Instance = null;
#endif
    }
}