using TMPro;

namespace Accelib.Localization.Architecture
{
    /// <summary>
    /// 언어 변경 이벤트 수신자
    /// </summary>
    public interface ILocaleChangedEventListener
    {
        // 언어 키
        public string LocaleKey { get; }
        
        // 언어 변경시 호출될 메소드
        public void OnLocaleUpdated(string localizedString, LocaleFontData fontAsset);
    }
}