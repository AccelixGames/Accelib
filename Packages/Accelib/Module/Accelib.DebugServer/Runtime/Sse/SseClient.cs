#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Accelib.DebugServer
{
    /// <summary>
    /// SSE 클라이언트 연결을 관리한다. 이벤트를 text/event-stream 형식으로 전송한다.
    /// </summary>
    internal sealed class SseClient : IDisposable
    {
        private readonly HttpListenerResponse _response;
        private readonly Stream _outputStream;
        private readonly string[] _filters;
        private bool _disposed;

        public bool IsConnected => !_disposed;

        internal SseClient(HttpListenerResponse response, string[] filters)
        {
            _response = response;
            _outputStream = response.OutputStream;
            _filters = filters;
        }

        /// <summary>
        /// 이벤트 타입이 이 클라이언트의 필터에 맞는지 확인한다.
        /// </summary>
        public bool AcceptsEvent(DebugEvent evt)
        {
            if (_filters == null || _filters.Length == 0) return true;
            for (var i = 0; i < _filters.Length; i++)
            {
                if (string.Equals(_filters[i], evt.EventType, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// SSE 형식으로 이벤트를 전송한다. 실패 시 연결을 닫는다.
        /// </summary>
        public bool TrySendEvent(DebugEvent evt)
        {
            if (_disposed) return false;
            try
            {
                var sb = new StringBuilder();
                sb.Append("event: ").Append(evt.EventType).Append('\n');
                sb.Append("data: ").Append(evt.DataJson).Append('\n');
                sb.Append('\n');

                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                _outputStream.Write(bytes, 0, bytes.Length);
                _outputStream.Flush();
                return true;
            }
            catch
            {
                Dispose();
                return false;
            }
        }

        /// <summary>
        /// 원시 바이트를 직접 전송한다. 하트비트 등에 사용.
        /// </summary>
        public bool WriteRaw(byte[] bytes)
        {
            if (_disposed) return false;
            try
            {
                _outputStream.Write(bytes, 0, bytes.Length);
                _outputStream.Flush();
                return true;
            }
            catch
            {
                Dispose();
                return false;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { _outputStream?.Close(); }
            catch { /* ignored */ }

            try { _response?.Close(); }
            catch { /* ignored */ }
        }
    }
}
#endif
