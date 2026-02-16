using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Module.Localization.Helper
{
    /// <summary>
    /// 두 언어 간 토글하는 컴포넌트.
    /// </summary>
    public class SimpleLocaleToggle : MonoBehaviour
    {
        [Tooltip("변경할 언어")]
        [SerializeField] private SystemLanguage languageA = SystemLanguage.Korean;
        [SerializeField] private SystemLanguage languageB = SystemLanguage.Korean;

        [Button("언어 토글하기")]
        public void ToggleLanguage()
        {
            LocalizationSingleton.ChangeLanguage(LocalizationSingleton.Instance.CurrLang == languageA
                ? languageB
                : languageA);
        }
    }
}
