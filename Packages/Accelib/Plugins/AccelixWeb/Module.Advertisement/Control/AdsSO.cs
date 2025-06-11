using Accelib.AccelixWeb.Module.Advertisement.Model;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.AccelixWeb.Module.Advertisement.Control
{
    [CreateAssetMenu(fileName = "AdsSO", menuName = "AccelibAIT/AdsSO", order = 0)]
    public class AdsSO : ScriptableObject
    {
        [field: Header("Option")]
        [field: SerializeField] public AdsType AdsType { get; private set; }
        [SerializeField] private bool autoReload = true;
        [SerializeField, TextArea] private string androidID;
        [SerializeField, TextArea] private string iosID;
        
        public string ID => AppInTossNative.GetPlatformOS() == "ios" ? iosID : androidID;

        [Header("State")]
        [SerializeField, ReadOnly] private AdsManager manager;
        [SerializeField, ReadOnly] private GameObject handlerInstance;
        private IAdsHandler _handler;
        
        private void Awake()
        {
            manager = null;
            _handler = null;
            handlerInstance = null;
        }

        internal void Initialize(AdsManager adsManager)
        {
            manager = adsManager;
            
            if (autoReload) Load();
        }

        internal void OnLoad(bool success)
        {
            // if(_handler == null) Debug.LogError($"[OnLoad({success})] Handler is null");
            _handler?.OnLoad(success);
        }

        internal void OnEvent(string code)
        {
            // if(_handler == null) Debug.LogError($"[OnEvent({code})] Handler is null");
            _handler?.OnEvent(code);

            if (code == AdsCode.Dismissed && autoReload)
                DelayedLoad().Forget();
        }

        private async UniTaskVoid DelayedLoad()
        {
            await UniTask.DelayFrame(5);
            Load();
        }

        internal void OnShow(bool success)
        {
            // if(_handler == null) Debug.LogError($"[OnShow({success})] Handler is null");
            _handler?.OnShow(success);
        }

        public void Register(IAdsHandler handler)
        {
            _handler = handler;
            handlerInstance = handler.GetInstance();
        }

        public void Unregister(IAdsHandler handler)
        {
            if (handler == _handler)
            {
                _handler = null;
                handlerInstance = null;
            }
        }
        
        public void Load(IAdsHandler handler)
        {
            Register(handler);
            Load();
        }
        
        public void Load()
        {
            if (!manager)
            {
                Debug.LogError($"AdsSO({AdsType}) not initialized({ID})");
                return;
            }
            
            manager.RequestAds(ID, AdsType, true);
        }

        public void Show()
        {
            if (!manager)
            {
                Debug.LogError($"AdsSO({AdsType}) not initialized({ID})");
                return;
            }
            
            manager.RequestAds(ID, AdsType, false);
        }

        // public void Mute(bool mute) => manager?.OnMute(mute);
    }
}