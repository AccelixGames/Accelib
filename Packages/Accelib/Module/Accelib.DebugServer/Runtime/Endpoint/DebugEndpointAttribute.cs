#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;

namespace Accelib.DebugServer
{
    /// <summary>
    /// HTTP 디버그 엔드포인트를 선언하는 어트리뷰트.
    /// DebugServerBase가 리플렉션으로 자동 탐색하여 라우트를 등록한다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DebugEndpointAttribute : Attribute
    {
        public string HttpMethod { get; }
        public string Route { get; }
        public string Description { get; }
        public string Category { get; set; }

        public DebugEndpointAttribute(string httpMethod, string route, string description)
        {
            HttpMethod = httpMethod.ToUpperInvariant();
            Route = route;
            Description = description;
            Category = string.Empty;
        }
    }
}
#endif
