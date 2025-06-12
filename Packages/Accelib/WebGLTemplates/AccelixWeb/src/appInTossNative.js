import { share, getTossShareLink, generateHapticFeedback, getSafeAreaInsets } from '@apps-in-toss/web-framework';
import { getDeviceId, getOperationalEnvironment, getTossAppVersion, isMinVersionSupported, getPlatformOS, getSchemeUri, getLocale } from '@apps-in-toss/web-framework';

window.getDeviceId = () => {
  try {
    return getDeviceId();
  } catch (e) {
    console.error('DeviceID 가져오기 실패: ', e);
    return "unknown";
  }
};

window.getOperationalEnvironment  = () => {
  try {
    // 'toss' | 'sandbox'
    return getOperationalEnvironment ();
  } catch (e) {
    console.error('운영환경 가져오기 실패: ', e);
    return "unknown";
  }
};

window.getTossAppVersion = () => {
  try {
    return getTossAppVersion();
  } catch (e) {
    console.error('앱인토스 버전 가져오기 실패: ', e);
    return "unknown";
  }
};

window.getPlatformOS = () => {
  try {
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
    return getSchemeUri();
  } catch (e) {
    console.error('스킴URI 가져오기 실패: ', e);
    return "unknown";
  }
};

window.getLocale  = () => {
  try {
    return getLocale();
  } catch (e) {
    console.error('스킴URI 가져오기 실패: ', e);
    return "unknown";
  }
};

window.handleShare = async (msg, deepLink) => {
  try {
    let finalMsg = msg;

    if (msg.includes('{link}')) {
      const link = await getTossShareLink(deepLink);
      finalMsg = msg.replace('{link}', link);
    }

    await share({ message: finalMsg });
  } catch (e) {
    console.error('공유 실패: ', e);
  }
};

window.getTossShareLink = async (deepLink) => {
  try {
    return await getTossShareLink(deepLink);
    // canvas?.unityInstance?.SendMessage?.(unityCallerName, unityMethod, link);
  } catch (e) {
    console.error('공유 링크 가져오기 실패: ', e);
    return "unknown";
  }
}

window.handleHapticFeedback = (typeName) => {
  try {
    generateHapticFeedback({type : typeName});
  } catch (e) {
    // console.error('진동발생 실패: ', e);
  }
};

window.getSafeAreaInsets = () => {
  try {
    const inset = getSafeAreaInsets();
    const json = JSON.stringify(inset);
    return json;
  } catch (e) {
    return "{}";
  }
};