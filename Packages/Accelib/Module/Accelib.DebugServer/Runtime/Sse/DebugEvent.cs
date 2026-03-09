#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Accelib.DebugServer
{
    /// <summary>
    /// 디버그 이벤트 데이터. SSE 스트림 및 이벤트 버퍼에 사용된다.
    /// </summary>
    public readonly struct DebugEvent
    {
        public readonly string EventType;
        public readonly string DataJson;
        public readonly float Timestamp;

        public DebugEvent(string eventType, string dataJson, float timestamp)
        {
            EventType = eventType;
            DataJson = dataJson;
            Timestamp = timestamp;
        }
    }
}
#endif
