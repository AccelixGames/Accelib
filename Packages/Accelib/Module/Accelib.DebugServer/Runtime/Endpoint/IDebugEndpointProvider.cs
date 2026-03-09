#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Accelib.DebugServer
{
    /// <summary>
    /// 디버그 서버 엔드포인트 제공자 마커 인터페이스.
    /// DebugServerCore가 GetComponentsInChildren으로 탐색하여 [DebugEndpoint] 메서드를 자동 등록한다.
    /// </summary>
    public interface IDebugEndpointProvider { }
}
#endif
