#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Accelib.DebugServer
{
    /// <summary>
    /// 디버그 이벤트 버스. 게임 시스템에서 이벤트를 발행하고, SSE 클라이언트에 전달한다.
    /// Publish()는 모든 스레드에서 안전하게 호출 가능하다.
    /// </summary>
    public sealed class DebugEventBus
    {
        private const int RingBufferCapacity = 256;

        private readonly ConcurrentQueue<DebugEvent> _pendingEvents = new ConcurrentQueue<DebugEvent>();

        // 링 버퍼 (메인 스레드 전용)
        private readonly DebugEvent[] _ringBuffer = new DebugEvent[RingBufferCapacity];
        private int _ringHead;
        private int _ringCount;

        public int RecentEventCount => _ringCount;

        /// <summary>
        /// 이벤트를 발행한다. 모든 스레드에서 호출 가능.
        /// </summary>
        public void Publish(string eventType, string dataJson, float timestamp)
        {
            var evt = new DebugEvent(eventType, dataJson, timestamp);
            _pendingEvents.Enqueue(evt);
        }

        /// <summary>
        /// 메인 스레드에서 보류 중인 이벤트를 하나 가져온다. 동시에 링 버퍼에 기록된다.
        /// </summary>
        internal bool TryDequeueEvent(out DebugEvent evt)
        {
            if (!_pendingEvents.TryDequeue(out evt))
                return false;

            // 링 버퍼에 기록
            _ringBuffer[_ringHead] = evt;
            _ringHead = (_ringHead + 1) % RingBufferCapacity;
            if (_ringCount < RingBufferCapacity) _ringCount++;

            return true;
        }

        /// <summary>
        /// 링 버퍼에 저장된 최근 이벤트 목록을 반환한다. 메인 스레드 전용.
        /// </summary>
        internal List<DebugEvent> GetRecentEvents()
        {
            var result = new List<DebugEvent>(_ringCount);
            if (_ringCount == 0) return result;

            var start = _ringCount < RingBufferCapacity ? 0 : _ringHead;
            for (var i = 0; i < _ringCount; i++)
            {
                var idx = (start + i) % RingBufferCapacity;
                result.Add(_ringBuffer[idx]);
            }

            return result;
        }

        /// <summary>
        /// 안전하게 이벤트를 발행한다. 서버가 비활성이면 무시한다. 메인 스레드 전용.
        /// </summary>
        public static void PublishSafe(string eventType, string dataJson)
        {
            if (!DebugServerCore.TryGetInstance(out var server)) return;
            server.EventBus.Publish(eventType, dataJson, Time.realtimeSinceStartup);
        }
    }
}
#endif
