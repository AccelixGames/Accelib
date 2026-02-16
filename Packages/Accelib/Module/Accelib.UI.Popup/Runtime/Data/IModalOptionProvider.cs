namespace Accelib.UI.Popup.Runtime.Data
{
    public interface IModalOptionProvider
    {
        public string Title { get; }
        public string Desc { get; }
        public object[] DescParams { get; }
        public string Ok { get; }
        public string Ng { get; }
    }
}