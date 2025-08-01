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
                _adsDict.Add(ads.ID, ads);
                ads.Initialize(this);
            }
        }

        public void RequestAds(string unitId, AdsType adsType, bool isLoadMethod) => AdsRouter.RequestAds(gameObject.name, unitId, adsType, isLoadMethod);

        public void OnLoad(string json)
        {
            var resp = ParseResponse(json);
            if (!_adsDict.TryGetValue(resp.unitId, out var ads))
            {
                // Debug.LogError($"[UnitId({resp.unitId})와 일치하는 광고객체를 찾을 수 없습니다] {json}", this);
                return;
            }

            // Debug.Log($"[OnLoad] {json}");
            ads.OnLoad(resp.code == AdsCode.Loaded);
        }

        public void OnEvent(string json)
        {
            var resp = ParseResponse(json);
            if (!_adsDict.TryGetValue(resp.unitId, out var ads))
            {
                // Debug.LogError($"[UnitId({resp.unitId})와 일치하는 광고객체를 찾을 수 없습니다] {json}", this);
                return;
            }
            
            // Debug.Log($"[OnEvent] {json}");
            ads.OnEvent(resp.code);
        }

        public void OnShow(string json)
        {
            var resp = ParseResponse(json);
            if (!_adsDict.TryGetValue(resp.unitId, out var ads))
            {
                // Debug.LogError($"[UnitId({resp.unitId})와 일치하는 광고객체를 찾을 수 없습니다] {json}", this);
                return;
            }
            
            // Debug.Log($"[OnShow] {json}");
            ads.OnShow(resp.code == AdsCode.Requested);
        }

        // public void OnMute(bool mute)
        // {
        //     AudioListener.pause = mute;
        // }

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
                    type = AdsType.Interstitial.ToString(),
                    unitId = "unknown",
                    code = AdsCode.Failed,
                    message = $"파싱 실패: {response}"
                };
            }
        }
    }
}
#endif