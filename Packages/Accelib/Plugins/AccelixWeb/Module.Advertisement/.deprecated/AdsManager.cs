#if ACCELIB_AIT
using System;
using System.Collections.Generic;
using Accelib.AccelixWeb.Module.Advertisement.Model;
using Newtonsoft.Json;
using UnityEngine;

namespace Accelib.AccelixWeb.Module.Advertisement.Control
{
    // 기능
    public class AdsManager : MonoBehaviour
    {
        [Header("Option")]
        [SerializeField] private List<AdsSO> adsList;
        private Dictionary<string , AdsSO> _adsDict;
        
        #if UNITY_EDITOR
        [Header("Editor Test Option")]
        public AdsResponse testResponse;
        #endif

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _adsDict = new Dictionary<string, AdsSO>();
            
            // 광고 초기화
            foreach (var ads in adsList)
            {
                ads.Initialize(this);
                _adsDict.Add(ads.ID, ads);
            }
        }

        /// <summary>광고 로드</summary>
        internal void LoadAds(AdsType type, string adsId) => AppInTossNative.CallAds(gameObject.name, adsId, true, type == AdsType.Interstitial);
        /// <summary>광고 보여주기</summary>
        internal void ShowAds(AdsType type, string adsId) => AppInTossNative.CallAds(gameObject.name, adsId, false, type == AdsType.Interstitial);

        // 이벤트 함수 (웹으로부터 전달 받기)
        public void OnLoadRewarded(string json)
        {
            var resp = ParseResponse(json);
            if (_adsDict.TryGetValue(resp.unitId, out var ads))
            {
                if (!resp.success)
                {
                    ads.OnLoaded?.Invoke(false);
                    ads.OnRewarded?.Invoke(false);
                }
                else
                {
                    // 로드 성공
                    if (resp.code == "loaded")
                        ads.OnLoaded?.Invoke(true);
                    // 리워드 성공
                    else if (resp.code == "userEarnedReward") 
                        ads.OnRewarded?.Invoke(true);
                }
            }
            else
            {
                Debug.LogError($"UnitID를 찾을 수 없습니다({resp.unitId}): {json}");
            }

            Debug.Log($"OnLoadRewarded: {json}");
        }
        
        // // 이벤트 함수 (웹으로부터 전달 받기)
        public void OnLoadInterstitial(string json)
        {
            var resp = ParseResponse(json);
            if (_adsDict.TryGetValue(resp.unitId, out var ads))
            {
                if (!resp.success)
                {
                    ads.OnLoaded?.Invoke(false);
                }
                else
                {
                    // 로드 성공
                    if (resp.code == "loaded")
                        ads.OnLoaded?.Invoke(true);
                }
            }
            
            Debug.Log($"OnLoadInterstitial: {json}");
        }

        // 이벤트 함수 (웹으로부터 전달 받기)
        public void OnShowRewarded(string json)
        {
            Debug.Log($"OnShowRewarded: {json}");
        }
        
        // 이벤트 함수 (웹으로부터 전달 받기)
        public void OnShowInterstitial(string json)
        {
            Debug.Log($"OnShowInterstitial: {json}");
        }

        private AdsResponse ParseResponse(string response)
        {
            try
            {
                return JsonConvert.DeserializeObject<AdsResponse>(response);
            }
            catch (Exception)
            {
                return new AdsResponse
                {
                    unitId = "-",
                    success = false,
                    code = "FailedToParseResponse",
                    message = $"파싱 실패: {response}"
                };
            }
        }
    }
}
#endif