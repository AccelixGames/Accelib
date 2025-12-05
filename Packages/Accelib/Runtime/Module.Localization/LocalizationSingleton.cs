using System.Collections.Generic;
using System.Linq;
using Accelib.Core;
using Accelib.Extensions;
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
            if (isInitialized) return;
            
            var systemLang = Application.systemLanguage;
            // Deb.Log($"시스템 언어: {systemLang}, 저장된 언어: {(SystemLanguage)currLangId.Value}");
            
            // 저장된 언어가 없다면,
            if(!IsSupportedLang(CurrLang))
                // 현재 시스템 언어를 가져옴
                currLangId.SetValue((int)systemLang);

            // 지원하는 언어가 아닐 경우, 
            if (!IsSupportedLang(CurrLang))
                // 첫번째 언어로 설정
                currLangId.Value = (int)locales[0].Language;

            // 로케일 변경
            UpdateLocale(true);

            isInitialized = true;
        }

        /// <summary>언어를 변경한다.</summary>
        private void ChangeLanguage(SystemLanguage language)
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
            UpdateLocale(false);
        }

        /// <summary>키값에 알맞는 로컬라이징 문자열을 가져온다.</summary>
        private string GetLocalizedString(string key, Object ctx = null)
        {
            if (currLocale?.TextDict == null)
            {
                Deb.LogWarning($"Locale이 또는 TextDict이 아직 로드되지 않았습니다. 현재 로케일({currLocale})", ctx);
                return NullString;
            }
            
            // 값 가져오기 실패할 경우,
            if (string.IsNullOrEmpty(key) || !currLocale.TextDict.TryGetValue(key, out var result))
            {
                // NULL 리턴
                Deb.LogWarning($"키({key})의 로컬라이징 값을 가져올 수 없습니다. 현재 언어({CurrLang})", ctx);
                return NullString;
            }
            
            return result;
        }

        // 로케일 값을 업데이트 한다.
        private void UpdateLocale(bool isInit)
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
                // 리스너가 Enable 이벤트를 받지 않는다면, 넘기기
                if (isInit && !listener.LoadOnEnable) continue;
                // 리스너의 키가 비어있다면, 종료
                if (string.IsNullOrEmpty(listener.LocaleKey)) continue;
                
                // 로케일 문자열 가져와서,
                var localizedString = GetLocalizedString(listener.LocaleKey, monoBehaviour);
                // 업데이트 해주기
                listener.OnLocaleUpdated(localizedString, GetFontData(listener.FontIndex));
            } 
        }

        private LocaleFontData GetFontData(int id = 0)
        {
            var list = currLocale.FontDataList;
            return list.GetOrDefault(id, list[0]);
        }

        public static void ChangeLanguageStatic(SystemLanguage language, Object ctx = null)
        {
            if (!Instance)
            {
                Deb.LogWarning("Instance가 null입니다.", ctx);
                return;
            }
            
            Instance.Init();
            Instance.ChangeLanguage(language);
        }
        
        public static string GetLocalizedStringStatic(string key, Object ctx = null)
        {
            if (!Instance)
            {
                Deb.LogWarning("Instance가 null입니다.", ctx);
                return NullString;
            }
            
            Instance.Init();
            return Instance.GetLocalizedString(key, ctx);
        }

        public static LocaleFontData GetFontAssetStatic(int id = 0, Object ctx = null)
        {
            if (!Instance)
            {
                Deb.LogWarning("Instance가 null입니다.", ctx);
                return null;
            }
            
            // 인스턴스 초기화
            Instance.Init();
            
            // 반환
            return Instance.GetFontData(id);
        }
        
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitSingleton() => Initialize();
#endif
    }
}