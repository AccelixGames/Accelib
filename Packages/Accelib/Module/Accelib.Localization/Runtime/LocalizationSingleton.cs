using System.Collections.Generic;
using System.Linq;
using Accelib.Core;
using Accelib.Module.Localization.Architecture;
using Accelib.Module.Localization.Model;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Module.Localization
{
    /// <summary>
    /// 로컬라이제이션 싱글톤. 언어 변경 및 로컬라이징 문자열 조회를 관리한다.
    /// </summary>
    public class LocalizationSingleton : MonoSingleton<LocalizationSingleton>
    {
        public const string NullString = "@null@";
        private const SystemLanguage DefaultLanguage = SystemLanguage.English;
        private const string LangueSaveKey =
#if UNITY_EDITOR
            "Editor.Accelix.Localization.Key";
#else
            "Accelix.Localization.Key";
#endif

        [Title("# 연결")]
        [SerializeField] private List<LocaleSO> locales;

        [Title("미리보기")]
        [ShowInInspector, ReadOnly] private bool _isInitialized = false;
        [ShowInInspector, ReadOnly] private SystemLanguage _currLanguage;
        [ShowInInspector, ReadOnly] private LocaleSO _currLocale = null;

        public SystemLanguage CurrLang => _currLanguage;
        private bool IsSupportedLang(SystemLanguage lang) => locales.Any(x => x.Language == lang);
        private LocaleSO GetLocale(SystemLanguage lang) => locales.FirstOrDefault(x => x.Language == lang) ?? locales[0];

        private void Internal_Init()
        {
            if (_isInitialized) return;

            // 저장된 언어 가져오기. 기본값은 시스템 언어.
            _currLanguage = (SystemLanguage)PlayerPrefs.GetInt(LangueSaveKey, (int)Application.systemLanguage);

            // 저장된 언어가 지원되는 언어가 아니라면,
            if (!IsSupportedLang(_currLanguage))
                // 기본 언어로 설정
                _currLanguage = DefaultLanguage;

            // 기본 언어가 지원하는 언어가 아닐 경우,
            if (!IsSupportedLang(_currLanguage))
                // 첫번째 언어로 설정
                _currLanguage = locales[0].Language;

            // 로케일 변경
            _currLocale = GetLocale(_currLanguage);
            PlayerPrefs.SetInt(LangueSaveKey, (int)_currLanguage);

            // 초기화 완료
            _isInitialized = true;
        }

        /// <summary>언어를 변경한다.</summary>
        private void Internal_ChangeLanguage(SystemLanguage language)
        {
            // 동일한 언어로 변경하려고 할 경우, 종료
            if(_currLanguage == language) return;

            // 지원하는 언어가 아닐 경우, 종료
            if (!IsSupportedLang(language))
            {
                Debug.LogWarning($"{language}는 지원하는 언어가 아닙니다.");
                return;
            }

            // 로케일 변경
            _currLanguage = language;
            _currLocale = GetLocale(language);
            PlayerPrefs.SetInt(LangueSaveKey, (int)_currLanguage);

            // 변경 이벤트 발생
            foreach (var monoBehaviour in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                // 로케일 변경 이벤트 리스너 가져오기. 없다면 넘기기
                if (!monoBehaviour.TryGetComponent<ILocaleChangedEventListener>(out var listener)) continue;
                // 리스너가 비활 상태라면, 넘기기
                if (!listener.IsEnabled) continue;
                // 리스너가 Enable 이벤트를 받지 않는다면, 넘기기
                if (!listener.LoadOnEnable) continue;
                // 리스너의 키가 비어있다면, 종료
                if (string.IsNullOrEmpty(listener.LocaleKey)) continue;

                // 로케일 문자열 가져와서,
                var localizedString = Internal_GetLocalizedString(listener.LocaleKey, monoBehaviour);
                // 업데이트 해주기
                listener.OnLocaleUpdated(localizedString);
            }
        }

        /// <summary>키값에 알맞는 로컬라이징 문자열을 가져온다.</summary>
        private string Internal_GetLocalizedString(string key, Object ctx = null)
        {
            if (!_currLocale)
            {
                return NullString;
            }

            // 값 가져오기 실패할 경우,
            if (string.IsNullOrEmpty(key) || !_currLocale.TryGetValue(key, out var result))
            {
                // NULL 리턴
                return NullString;
            }

            return result;
        }

        /// <summary>언어를 변경한다.</summary>
        public static void ChangeLanguage(SystemLanguage language, Object ctx = null)
        {
            var inst = GetInstanceSafe();
            if (!inst)
            {
                Debug.LogWarning("Instance가 null입니다.", ctx);
                return;
            }

            inst.Internal_Init();
            inst.Internal_ChangeLanguage(language);
        }

        public static string GetLocalizedString(LocaleKey locale, Object ctx = null) => GetLocalizedString(locale.key, ctx);
        
        /// <summary>키값에 알맞는 로컬라이징 문자열을 가져온다.</summary>
        public static string GetLocalizedString(string key, Object ctx = null)
        {
            var inst = GetInstanceSafe();
            if (!inst)
            {
                Debug.LogWarning("Instance가 null입니다.", ctx);
                return NullString;
            }

            inst.Internal_Init();
            return inst.Internal_GetLocalizedString(key, ctx);
        }

        /// <summary>폰트 데이터를 가져온다.</summary>
        public static LocaleFontData GetFontData(int id, Object ctx = null)
        {
            var inst = GetInstanceSafe();
            if (!inst)
            {
                Debug.LogWarning("Instance가 null입니다.", ctx);
                return null;
            }

            inst.Internal_Init();
            return inst._currLocale.GetFontData(id);
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitSingleton() => Initialize();
#endif
    }
}
