using Accelib.Logging;
using Accelib.Module.Localization.Architecture;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using AYellowpaper.SerializedCollections;

namespace Accelib.Module.Localization.Helper
{
    [RequireComponent(typeof(Image))]
    public class LocalizedImage : MonoBehaviour, ILocaleChangedEventListener
    {
        [field:SerializeField, ReadOnly] public Image Image { get; private set; }
        [SerializeField] SerializedDictionary<SystemLanguage, Sprite> localizedImages = new ();
        
        public string LocaleKey { get; }
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

        [Button("Reload", EButtonEnableMode.Playmode)]
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
                    Deb.Log($"LocalizedImage에서 지원하지 않는 언어{language} 입니다. 0번째 이미지로 설정합니다. ");
                }
                else
                {
                    Image.sprite = null;
                    
                    Deb.Log($"LocalizedImage에서 지원하지 않는 언어{language} 입니다. 설정할 수 있는 이미지가 없습니다.");
                }
            }
        }

        public void OnLocaleUpdated(string localizedString, LocaleFontData fontAsset)
        {
            Load();
        }
    }
}