using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.Localization.Helper
{
    public class SimpleLocaleToggle : MonoBehaviour
    {
        [Tooltip("변경할 언어")]
        [SerializeField] private SystemLanguage languageA = SystemLanguage.Korean;
        [SerializeField] private SystemLanguage languageB = SystemLanguage.Korean;
        
        [Button("언어 토글하기", EButtonEnableMode.Playmode)]
        public void ToggleLanguage()
        {
            LocalizationSingleton.ChangeLanguageStatic(LocalizationSingleton.Instance.CurrLang == languageA
                ? languageB
                : languageA);
        }
    }
}