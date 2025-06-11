import { GoogleAdMob } from "@apps-in-toss/web-framework";

// 유니티 메서드명 상수
const UNITY_CALLBACK = {
  ON_LOAD: "OnLoad",
  ON_EVENT: "OnEvent",
  ON_SHOW: "OnShow",
};

// 공통 응답 타입 상수
const CODE = {
  FAILED: "failed",
  UNKNOWN: "unknown"
};

const ADS_TYPE = {
  INTERSTITIAL: "interstitial",
  REWARDED: "rewarded",
}

function sendUnityMsg(unityCallerName, unityMethod, type, unitId, code, message) 
{
  const json = JSON.stringify({
      type : type,
      unitId: unitId ?? CODE.UNKNOWN,
      code: code ?? CODE.UNKNOWN,
      message: message ?? CODE.UNKNOWN,
    });

  canvas?.unityInstance?.SendMessage?.(unityCallerName, unityMethod, json);
}

function requestAds({unityCallerName, apiMethod, adsType, unitId, unityCallback}) {
  
  // 광고ID 캐싱
  const id = unitId ?? CODE.UNKNOWN;
  
  try{  
    // 미지원 핸들링
    if (apiMethod.isSupported() !== true) {
      sendUnityMsg(unityCallerName, unityCallback, adsType, id, CODE.FAILED, `[Error] ${apiMethod} is not supported.`);
      return;
    }

    // 광고 함수 호출
    apiMethod({
      // 옵션 설정
      options: { adUnitId: id },

      // 이벤트 핸들러 설정
      onEvent: (event) => { 
        if(event.type == "loaded") { sendUnityMsg(unityCallerName, unityCallback, adsType, id, event.type, ""); }
        else if(event.type == "requested") { sendUnityMsg(unityCallerName, unityCallback, adsType, id, event.type, ""); }
        else { sendUnityMsg(unityCallerName, UNITY_CALLBACK.ON_EVENT, adsType, id, event.type, ""); }
      },

      // 에러 핸들러 설정
      onError: (error) => { sendUnityMsg(unityCallerName, unityCallback, adsType, id, CODE.FAILED, `[${error?.code ?? CODE.UNKNOWN}] ${error?.message ?? "Unknown error"}`); }
    });
  }catch (ex) {
    sendUnityMsg(unityCallerName, unityCallback, adsType, id, CODE.FAILED, `[Exception] ${ex}`);
  }
}

// 광고 관련 함수 정의
window.aitAds = {
  // 전면 광고 로드
  loadInterstitial: (unityCallerName, unitId) => {
    requestAds({
      unityCallerName,
      apiMethod: GoogleAdMob.loadAdMobInterstitialAd,
      adsType: ADS_TYPE.INTERSTITIAL,
      unitId: unitId,
      unityCallback: UNITY_CALLBACK.ON_LOAD,
    });
  },

    // 리워드 광고 로드
  loadRewarded: (unityCallerName, unitId) => {
    requestAds({
      unityCallerName,
      apiMethod: GoogleAdMob.loadAdMobRewardedAd,
      adsType: ADS_TYPE.REWARDED,
      unitId: unitId,
      unityCallback: UNITY_CALLBACK.ON_LOAD,
    });
  },

  // 전면 광고 표시
  showInterstitial: (unityCallerName, unitId) => {
    requestAds({
      unityCallerName,
      apiMethod: GoogleAdMob.showAdMobInterstitialAd,
      adsType: ADS_TYPE.INTERSTITIAL,
      unitId: unitId,
      unityCallback: UNITY_CALLBACK.ON_SHOW,
    });
  },

  // 리워드 광고 표시
  showRewarded: (unityCallerName, unitId) => {
    requestAds({
      unityCallerName,
      apiMethod: GoogleAdMob.showAdMobRewardedAd,
      adsType: ADS_TYPE.REWARDED,
      unitId: unitId,
      unityCallback: UNITY_CALLBACK.ON_SHOW,
    });
  },
};
