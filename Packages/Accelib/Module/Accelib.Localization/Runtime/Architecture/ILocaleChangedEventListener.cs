namespace Accelib.Module.Localization.Architecture
{
    /// <summary>
    /// 언어 변경 이벤트 수신자
    /// </summary>
    public interface ILocaleChangedEventListener
    {
        public string LocaleKey { get; }
        public int FontIndex { get; }

        public bool IsEnabled { get; }
        public bool LoadOnEnable { get; }

        /// <summary>언어 변경시 호출될 메소드</summary>
        public void OnLocaleUpdated(string localizedString);
    }
}
