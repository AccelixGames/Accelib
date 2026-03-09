#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Accelib.DebugServer
{
    /// <summary>
    /// 등록된 엔드포인트의 메타데이터. API 문서 자동 생성에 사용된다.
    /// </summary>
    public readonly struct DebugEndpointInfo
    {
        public readonly string HttpMethod;
        public readonly string Route;
        public readonly string Description;
        public readonly string Category;

        public DebugEndpointInfo(string httpMethod, string route, string description, string category)
        {
            HttpMethod = httpMethod;
            Route = route;
            Description = description;
            Category = category ?? string.Empty;
        }

        public DebugEndpointInfo(DebugEndpointAttribute attr)
            : this(attr.HttpMethod, attr.Route, attr.Description, attr.Category)
        {
        }
    }
}
#endif
