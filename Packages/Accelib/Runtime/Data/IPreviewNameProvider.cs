namespace Accelib.Data
{
    public interface IPreviewNameProvider
    {
#if UNITY_EDITOR
        public string EditorPreviewName { get; }
#endif
    }
}