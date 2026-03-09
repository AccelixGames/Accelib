#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Accelib.DebugServer
{
    /// <summary>
    /// 디버그 서버 엔드포인트 제공자 인터페이스.
    /// DebugServerCore가 GetComponentsInChildren으로 탐색하여 [DebugEndpoint] 메서드를 자동 등록한다.
    /// RoutePrefix와 CategoryName을 제공하여 라우트 prefix 자동 조합 및 카테고리 기본값을 설정한다.
    /// </summary>
    public interface IDebugEndpointProvider
    {
        /// <summary> 라우트 prefix. 예: "/api/cafe". [DebugEndpoint]의 Route 앞에 자동 결합된다. </summary>
        string RoutePrefix { get; }

        /// <summary> 카테고리명. 예: "카페". [DebugEndpoint]의 Category 기본값으로 사용된다. </summary>
        string CategoryName { get; }
    }
}
#endif
