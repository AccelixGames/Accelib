using System.Collections.Generic;
using System.Linq;
using Accelib.Core;
using Accelib.Localization.Architecture;
using Accelib.Logging;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Accelib.Localization
{
    public class LocalizationSingleton : MonoSingleton<LocalizationSingleton>
    {
        public const string NullString = "@null@";
        private static readonly string LangKey = $"{nameof(LocalizationSingleton)}-{nameof(currLanguage)}";
        
        // 지원하는 로케일 데이터들
        [SerializeField] private List<LocaleSO> locales;
        
        [Header("미리보기")]
        [SerializeField, ReadOnly] private SystemLanguage currLanguage = SystemLanguage.Unknown;
        [SerializeField, ReadOnly] private LocaleSO currLocale = null;

        private void Start()
        {
            // 저장된 언어 로드
            var lang = PlayerPrefs.GetInt(LangKey, (int)SystemLanguage.Unknown);
            currLanguage = (SystemLanguage)lang;
            
            // 저장된 언어가 없다면,
            if (currLanguage == SystemLanguage.Unknown)
            {
                // 현재 시스템 언어를 가져옴
                currLanguage = Application.systemLanguage;
            }

            // 지원하는 언어가 아닐 경우, 
            if (locales.All(x => x.Language != currLanguage))
            {
                // 첫번째 언어로 설정
                currLanguage = locales[0].Language;

                // 언어 저장
                PlayerPrefs.SetInt(LangKey, (int)currLanguage);
            }

            // 로케일 변경
            UpdateLocale();
        }

        /// <summary>언어를 변경한다.</summary>
        public void ChangeLanguage(SystemLanguage language)
        {
            // 동일한 언어로 변경하려고 할 경우, 종료
            if(currLanguage == language) return;
            
            // 지원하는 언어가 아닐 경우, 종료
            if (locales.All(x => x.Language != language))
            {
                Deb.LogWarning($"{language}는 지원하는 언어가 아닙니다.");
                return;
            }

            // 언어 로드
            currLanguage = language;
            
            // 언어 저장
            PlayerPrefs.SetInt(LangKey, (int)currLanguage);
            
            // 로케일 변경
            UpdateLocale();
        }

        public static string GetLocalizedStringStatic(string key) => Instance?.GetLocalizedString(key) ?? NullString;

        public static LocaleFontData GetFontAssetStatic() => Instance?.currLocale?.FontData;

        /// <summary>키값에 알맞는 로컬라이징 문자열을 가져온다.</summary>
        public string GetLocalizedString(string key)
        {
            if(currLocale == null)
                return NullString;
            
            // 값 가져오기 실패할 경우,
            if (!currLocale.TextDict.TryGetValue(key, out var result))
            {
                // NULL 리턴
                Deb.LogWarning($"키({key})의 로컬라이징 값을 가져올 수 없습니다. 현재 언어({currLanguage})");
                result = NullString;
            }
            
            return result;
        }

        // 로케일 값을 업데이트 한다.
        private void UpdateLocale()
        {
            // 로케일 변경
            currLocale = locales.FirstOrDefault(x => x.Language == currLanguage);
            
            // 변경 이벤트 발생
            foreach (var monoBehaviour in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                if(monoBehaviour.TryGetComponent<ILocaleChangedEventListener>(out var listener))
                    listener.OnLocaleUpdated(GetLocalizedString(listener.LocaleKey), currLocale?.FontData);    
            } 
        }
        
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => Instance = null;
#endif
    }
}