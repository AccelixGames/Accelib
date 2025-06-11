using Accelib.AccelixWeb.Module.Advertisement.Control.Emulator;
using Accelib.AccelixWeb.Module.Advertisement.Model;
using UnityEngine;

namespace Accelib.AccelixWeb.Module.Advertisement.Control
{
    public static class AdsRouter
    {
        private static AdsEmulator _emulator;
        
        public static bool IsAppInTossEnvironment()
        {
#if UNITY_EDITOR
            return false;
#else
            var appVersion = AppInTossNative.GetTossAppVersion();
            if (string.IsNullOrEmpty(appVersion) || appVersion == AppInTossNative.Unknown)
                return false;
            
            return true;
#endif
        }

        public static void RequestAds(string callerName, string unitId, AdsType adsType, bool isLoadMethod)
        {
            var isInterstitial = adsType == AdsType.Interstitial;
            
            // 앱인토스 환경이라면,
            if (IsAppInTossEnvironment())
            {
                // 함수 호출
                AppInTossNative.CallAds(callerName, unitId, isLoadMethod, isInterstitial);
                //Debug.Log($"AppInTossNative.CallAds({callerName}, {unitId}, {isLoadMethod}, {isInterstitial})");
            }
            // 아니라면,
            else
            {
                // 현재 에뮬레이터가 없다면,
                if (_emulator == null)
                {
                    // 런타임에서 찾아보고,
                    _emulator = Object.FindFirstObjectByType<AdsEmulator>(FindObjectsInactive.Exclude);
                    
                    // 그래도 없으면 생성
                    if (_emulator == null)
                    {
                        var prefab = Resources.Load<AdsEmulator>(nameof(AdsEmulator));
                        _emulator = Object.Instantiate(prefab);   
                    }
                }
                
                // 호출
                _emulator.CallAds(callerName, unitId, isLoadMethod, isInterstitial);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => _emulator = null;
    }
}