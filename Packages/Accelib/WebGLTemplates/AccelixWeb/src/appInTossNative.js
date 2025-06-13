import { share, getTossShareLink, generateHapticFeedback, getSafeAreaInsets } from '@apps-in-toss/web-framework';
import { getDeviceId, getOperationalEnvironment, getTossAppVersion, isMinVersionSupported, getPlatformOS, getSchemeUri, getLocale } from '@apps-in-toss/web-framework';
import { setIosSwipeGestureEnabled } from '@apps-in-toss/web-framework';

function isInAppInToss() {
  return typeof window.ReactNativeWebView !== 'undefined';
}

// IOS 스와이프 제스처 비활성화
if(isInAppInToss())
  setIosSwipeGestureEnabled({ isEnabled: false });

window.getDeviceId = () => {
  try {
    if(!isInAppInToss()) return "localhost";

    return getDeviceId();
  } catch (e) {
    console.error('DeviceID 가져오기 실패: ', e);
    return "unknown";
  }
};

window.getOperationalEnvironment  = () => {
  try {
    if(!isInAppInToss()) return "localhost";

    // 'toss' | 'sandbox'
    return getOperationalEnvironment ();
  } catch (e) {
    console.error('운영환경 가져오기 실패: ', e);
    return "unknown";
  }
};

window.getTossAppVersion = () => {
  try {
    if(!isInAppInToss()) return "localhost";

    return getTossAppVersion();
  } catch (e) {
    console.error('앱인토스 버전 가져오기 실패: ', e);
    return "unknown";
  }
};

window.getPlatformOS = () => {
  try {
    if(!isInAppInToss()) throw new Error();

    // 'ios' | 'android'
    return getPlatformOS();
  } catch (e) {
    const ua = navigator.userAgent || navigator.vendor || window.opera;

    if (/android/i.test(ua)) return "android";
    if (/iPad|iPhone|iPod/.test(ua) && !window.MSStream) return "ios";
    if (/Win/.test(ua)) return "windows";
    if (/Mac/.test(ua)) return "macos";
    if (/Linux/.test(ua)) return "linux";

    return "unknown";
  }
};

window.getSchemeUri  = () => {
  try {
    if(!isInAppInToss()) return "localhost";
    return getSchemeUri();
  } catch (e) {
    console.error('스킴URI 가져오기 실패: ', e);
    return "unknown";
  }
};

window.getLocale  = () => {
  try {
    if(!isInAppInToss()) return "unknown";
    return getLocale();
  } catch (e) {
    console.error('스킴URI 가져오기 실패: ', e);
    return "unknown";
  }
};

window.handleShare = async (msg, deepLink) => {
  try {
    if(isInAppInToss()){
      let finalMsg = msg;

      if (msg.includes('{link}')) {
        const link = await getTossShareLink(deepLink);
        finalMsg = msg.replace('{link}', link);
      }

      await share({ message: finalMsg });
    }
  } catch (e) {
    console.error('공유 실패: ', e);
  }
};

window.handleHapticFeedback = (typeName) => {
  try {
    if(!isInAppInToss()) return;
    generateHapticFeedback({type : typeName});
  } catch (e) {
    console.error('진동발생 실패: ', e);
  }
};

window.getSafeAreaInsets = () => {
  try {
    if(!isInAppInToss()) return "{top:0, bottom:0}";
    const inset = getSafeAreaInsets();
    const json = JSON.stringify(inset);
    return json;
  } catch (e) {
    return "{}";
  }
};