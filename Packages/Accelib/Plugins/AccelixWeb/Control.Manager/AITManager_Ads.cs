#if ACCELIB_AIT
using System;
using UnityEngine;

namespace Accelib.AccelixWeb.Control.Manager
{
    // 싱글톤
    public partial class AITManager_Ads : MonoBehaviour
    {
        public static AITManager_Ads Instance;
        
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    // 기능
    public partial class AITManager_Ads
    {
        [Header("Options")]
        [SerializeField, TextArea] private string interstitialId = "ca-app-pub-3940256099942544/1033173712";
        [SerializeField, TextArea] private string rewardedId = "ca-app-pub-3940256099942544/5224354917";
        
        public void LoadRewarded() => AppInTossNative.CallAds(gameObject.name, rewardedId, true, false);

        public void ShowRewarded() => AppInTossNative.CallAds(gameObject.name, rewardedId, false, false);
        
        public void OnLoadRewarded(string type)
        {
            Debug.Log($"OnLoadRewarded: {type}");
        }

        public void OnShowRewarded(string type)
        {
            Debug.Log($"OnShowRewarded: {type}");
        }
    }
}
#endif