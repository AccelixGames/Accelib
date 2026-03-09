#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.DebugServer
{
    /// <summary>
    /// IDebugEndpointProvider 기본 구현. 인스펙터에 도메인 정보를 자동 표시한다.
    /// 상속 불가한 클래스에서는 IDebugEndpointProvider를 직접 구현한다.
    /// </summary>
    public abstract class DebugEndpointProviderBase : MonoBehaviour, IDebugEndpointProvider
    {
        [ShowInInspector, ReadOnly, TitleGroup("도메인 정보")]
        public abstract string RoutePrefix { get; }

        [ShowInInspector, ReadOnly, TitleGroup("도메인 정보")]
        public abstract string CategoryName { get; }
    }
}
#endif
