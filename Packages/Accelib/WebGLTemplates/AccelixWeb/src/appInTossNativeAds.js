import { GoogleAdMob } from '@apps-in-toss/web-framework';

// 유니티 메서드명 상수
const UNITY_METHOD = {
  INTERSTITIAL_LOAD: "OnLoadInterstitial",
  INTERSTITIAL_SHOW: "OnShowInterstitial",
  REWARDED_LOAD: "OnLoadRewarded",
  REWARDED_SHOW: "OnShowRewarded"
};

// 공통 응답 타입 상수
const RESP = {
  ERROR: "error",
  EXCEPTION: "exception"
};

// 이벤트 전달 함수
function sendUnityMessage(unityCallerName, unityMethod, type, data = '') {
  canvas?.unityInstance?.SendMessage?.(unityCallerName, unityMethod, type, data);
}

// 공통 광고 로드 함수
function loadAd({ unityCallerName, unityMethod, apiFn, unitId }) {
  try {
    if (apiFn.isSupported() !== true) {
      sendUnityMessage(unityCallerName, unityMethod, RESP.ERROR, 'NotSupported');
      console.error(`${unityMethod}는 지원되지 않습니다.`);
      return;
    }

    apiFn({
      options: { adUnitId: unitId },
      onEvent: (event) => {
        // 모든 이벤트를 Unity로 전달
        sendUnityMessage(unityCallerName, unityMethod, event.type, event.data || '');
      },
      onError: (error) => {
        sendUnityMessage(unityCallerName, unityMethod, RESP.ERROR, error);
        console.error('광고 불러오기 실패', error);
      }
    });
  } catch (e) {
    sendUnityMessage(unityCallerName, unityMethod, RESP.EXCEPTION, '');
    console.error(e);
  }
}

// 공통 광고 표시 함수
function showAd({ unityCallerName, unityMethod, apiFn, unitId }) {
  try {
    if (apiFn.isSupported() !== true) {
      sendUnityMessage(unityCallerName, unityMethod, RESP.ERROR, 'NotSupported');
      console.error(`${unityMethod}는 지원되지 않습니다.`);
      return;
    }

    apiFn({
      options: { adUnitId: unitId },
      onEvent: (event) => {
        if (event.type === 'requested') {
          sendUnityMessage(unityCallerName, unityMethod, event.type, '');
        }
      },
      onError: (error) => {
        sendUnityMessage(unityCallerName, unityMethod, RESP.ERROR, error);
        console.error('광고 보여주기 실패', error);
      }
    });
  } catch (e) {
    sendUnityMessage(unityCallerName, unityMethod, RESP.EXCEPTION, '');
    console.error(e);
  }
}

window.aitAds = {

// 전면 광고 로드
  loadInterstitial : (unityCallerName, unitId) => {
    loadAd({
      unityCallerName,
      unityMethod: UNITY_METHOD.INTERSTITIAL_LOAD,
      apiFn: GoogleAdMob.loadAdMobInterstitialAd,
      unitId
    });
  },

// 전면 광고 표시
  showInterstitial : (unityCallerName, unitId) => {
    showAd({
      unityCallerName,
      unityMethod: UNITY_METHOD.INTERSTITIAL_SHOW,
      apiFn: GoogleAdMob.showAdMobInterstitialAd,
      unitId
    });
  },

// 리워드 광고 로드
  loadRewarded : (unityCallerName, unitId) => {
    loadAd({
      unityCallerName,
      unityMethod: UNITY_METHOD.REWARDED_LOAD,
      apiFn: GoogleAdMob.loadAdMobRewardedAd,
      unitId
    });
  },

// 리워드 광고 표시
  showRewarded : (unityCallerName, unitId) => {
    showAd({
      unityCallerName,
      unityMethod: UNITY_METHOD.REWARDED_SHOW,
      apiFn: GoogleAdMob.showAdMobRewardedAd,
      unitId
    });
  }
}