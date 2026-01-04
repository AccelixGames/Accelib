namespace Accelib.Module.Localization.Helper.Formatter
{
    public interface ILocalizedFormatter
    {
        public object[] GetArgs();

        public void SetArgs(params object[] args);
    }
}