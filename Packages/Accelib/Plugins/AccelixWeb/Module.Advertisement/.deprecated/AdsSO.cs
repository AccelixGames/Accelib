using Accelib.AccelixWeb.Module.Advertisement.Control;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.AccelixWeb.Module.Advertisement.Model
{
    [CreateAssetMenu(fileName = "AdsSO", menuName = "AccelibAIT/AdsSO", order = 0)]
    public class AdsSO : ScriptableObject
    {
        [field: Header("Option")]
        [field: SerializeField] public AdsType AdsType { get; private set; }

        [SerializeField, TextArea] private string androidID;
        [SerializeField, TextArea] private string iosID;
        
        public string ID => AppInTossNative.GetPlatformOS() == "ios" ? iosID : androidID;

        [Header("State")]
        [SerializeField, ReadOnly] private AdsManager manager;

        internal UnityAction<bool> OnLoaded;
        internal UnityAction<bool> OnRewarded;
        
        private void Awake()
        {
            manager = null;
            OnLoaded = null;
            OnRewarded = null;
        }
        
        internal void Initialize(AdsManager adsManager)
        {
            manager = adsManager;
            UnSubscribeAll();
        }

        public void UnSubscribeAll()
        {
            OnLoaded = null;
            OnRewarded = null;
        }
        
        public void Load(UnityAction<bool> onLoaded)
        {
            if (!manager)
            {
                Debug.LogError($"AdsSO({AdsType}) not initialized({ID})");
                return;
            }
            
            if(onLoaded != null) OnLoaded += onLoaded;
            Load();
        }

        public void Show(UnityAction<bool> onRewarded)
        {
            if (!manager)
            {
                Debug.LogError($"AdsSO({AdsType}) not initialized({ID})");
                return;
            }
            
            if(onRewarded != null) OnRewarded += onRewarded;
            Show();
        }
        
        public void Load() => manager?.LoadAds(AdsType, ID);
        public void Show() => manager?.ShowAds(AdsType, ID);
    }
}