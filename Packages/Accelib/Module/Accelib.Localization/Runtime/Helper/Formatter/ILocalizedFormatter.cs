namespace Accelib.Module.Localization.Helper.Formatter
{
    /// <summary>
    /// 로컬라이즈된 문자열에 포맷 인자를 제공하는 인터페이스.
    /// </summary>
    public interface ILocalizedFormatter
    {
        public object[] GetArgs();

        public void SetArgs(params object[] args);
    }
}
