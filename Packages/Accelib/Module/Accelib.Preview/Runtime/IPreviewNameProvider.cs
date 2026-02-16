namespace Accelib.Preview
{
    /// <summary>에디터 프리뷰에 표시할 이름을 제공하는 인터페이스.</summary>
    public interface IPreviewNameProvider
    {
        public string EditorPreviewName { get; }
    }
}
