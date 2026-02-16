using Accelib.Module.Localization.Architecture;
using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;

namespace Accelib.Module.Localization.Helper
{
    /// <summary>
    /// 언어에 따라 이미지를 교체하는 컴포넌트.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class LocalizedImage : MonoBehaviour, ILocaleChangedEventListener
    {
        [field:SerializeField, ReadOnly] public Image Image { get; private set; }
        [SerializeField] SerializedDictionary<SystemLanguage, Sprite> localizedImages = new ();

        public string LocaleKey => string.Empty;
        public int FontIndex => 0;
        public bool IsEnabled => enabled;
        public bool LoadOnEnable => true;

        private void Start()
        {
            Image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            Load();
        }

        [Button("Reload")]
        public void Load()
        {
            Image ??= GetComponent<Image>();
            if (Image == null) return;

            var language = LocalizationSingleton.Instance.CurrLang;
            if(localizedImages.TryGetValue(language, out var sprite))
            {
                Image.sprite = sprite;
            }
            else
            {
                if (localizedImages.Count > 0 && localizedImages[0] != null)
                {
                    Image.sprite = localizedImages[0];
                    Debug.LogWarning($"LocalizedImage에서 지원하지 않는 언어{language} 입니다. 0번째 이미지로 설정합니다. ");
                }
                else
                {
                    Image.sprite = null;

                    Debug.LogWarning($"LocalizedImage에서 지원하지 않는 언어{language} 입니다. 설정할 수 있는 이미지가 없습니다.");
                }
            }
        }

        public void OnLocaleUpdated(string localizedString)
        {
            Load();
        }
    }
}
