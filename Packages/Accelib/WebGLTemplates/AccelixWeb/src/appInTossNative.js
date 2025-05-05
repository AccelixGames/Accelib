import { share, generateHapticFeedback, getTossAppVersion, getSafeAreaInsets} from '@apps-in-toss/web-framework';

window.handleShare = async (msg) => {
  try {
    await share({ message: msg });
  } catch (e) {
    console.error('공유 실패: ', e);
  }
};

window.handleHapticFeedback = (typeName) => {
  try {
    generateHapticFeedback({type : typeName});
  } catch (e) {
    // console.error('진동발생 실패: ', e);
  }
};

window.getTossAppVersion = () => {
  try {
    return getTossAppVersion();
  } catch (e) {
    console.error('앱인토스 버전 가져오기 실패: ', e);
    return "-";
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