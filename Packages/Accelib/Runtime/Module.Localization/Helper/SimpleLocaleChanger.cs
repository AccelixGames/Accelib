using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Module.Localization.Helper
{
    /// <summary>
    /// 언어를 변경해주는 심플한 도구.
    /// </summary>
    public class SimpleLocaleChanger : MonoBehaviour
    {
        [Tooltip("변경할 언어")]
        [SerializeField] private SystemLanguage language = SystemLanguage.Korean;
        
        [Button("언어 변경", ButtonSizes.Large)]
        public void ChangeLanguage() => LocalizationSingleton.ChangeLanguage(language);
    }
}