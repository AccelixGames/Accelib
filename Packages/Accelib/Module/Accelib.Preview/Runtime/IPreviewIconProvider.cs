#if ODIN_INSPECTOR
using Sirenix.OdinInspector;

namespace Accelib.Preview
{
    /// <summary>에디터 프리뷰에 표시할 SdfIcon을 제공하는 인터페이스.</summary>
    public interface IPreviewIconProvider
    {
        public SdfIconType EditorPreviewIcon { get; }
    }
}
#endif
