#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Accelib.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.DebugServer
{
    /// <summary>
    /// HTTP 디버그 서버 코어.
    /// 하위 IDebugEndpointProvider 컴포넌트에서 [DebugEndpoint] 메서드를 정의하면 자동으로 라우트가 등록된다.
    /// </summary>
    [DefaultExecutionOrder(1000)]
    [DisallowMultipleComponent]
    public sealed class DebugServerCore : MonoSingleton<DebugServerCore>
    {
        #region Fields

        [TitleGroup("Debug Server")]
        [SerializeField] private int port = 7860;

        [TitleGroup("Debug Server")]
        [SerializeField] private bool autoStart = true;

        [TitleGroup("디버깅")]
        [ShowInInspector, ReadOnly]
        private bool _isRunning;

        [TitleGroup("디버깅")]
        [ShowInInspector, ReadOnly]
        private int _registeredEndpointCount;

        private HttpListener _listener;
        private Thread _listenerThread;
        private ConcurrentQueue<RequestContext> _requestQueue;
        private float _startTime;

        private readonly List<RouteEntry> _routes = new List<RouteEntry>();
        private readonly List<DebugEndpointInfo> _endpointInfos = new List<DebugEndpointInfo>();

        #endregion

        #region Public Properties

        public bool IsRunning => _isRunning;
        public int Port => port;
        public int RegisteredEndpointCount => _registeredEndpointCount;

        #endregion

        #region Lifecycle

        protected override void Awake()
        {
            base.Awake();
            _requestQueue = new ConcurrentQueue<RequestContext>();
        }

        private void OnEnable()
        {
            if (autoStart) StartServer();
        }

        private void OnDisable()
        {
            StopServer();
        }

        private void Update()
        {
            // 메인 스레드에서 요청 처리
            while (_requestQueue.TryDequeue(out var ctx))
            {
                try
                {
                    ProcessRequest(ctx);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    try { ctx.RespondError(e.Message, 500); }
                    catch { /* 응답 전송 실패 무시 */ }
                }
            }

        }

        #endregion

        #region Server Control

        /// <summary>
        /// HTTP 서버를 시작한다.
        /// </summary>
        [TitleGroup("Debug Server")]
        [Button("Start Server"), ShowIf("@!_isRunning")]
        public void StartServer()
        {
            if (_isRunning) return;

            // 엔드포인트 등록
            RegisterAllEndpoints();

            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add($"http://localhost:{port}/");
                _listener.Prefixes.Add($"http://127.0.0.1:{port}/");
                _listener.Start();

                _isRunning = true;
                _startTime = Time.realtimeSinceStartup;

                // 백그라운드 스레드에서 요청 수신
                _listenerThread = new Thread(ListenLoop) { IsBackground = true };
                _listenerThread.Start();

                Debug.Log($"[DebugServer] Started on port {port} ({_registeredEndpointCount} endpoints)");
            }
            catch (Exception e)
            {
                Debug.LogError($"[DebugServer] Failed to start: {e.Message}");
                _isRunning = false;
            }
        }

        /// <summary>
        /// HTTP 서버를 중지한다.
        /// </summary>
        [TitleGroup("Debug Server")]
        [Button("Stop Server"), ShowIf("_isRunning")]
        public void StopServer()
        {
            if (!_isRunning) return;

            _isRunning = false;

            try
            {
                _listener?.Stop();
                _listener?.Close();
            }
            catch { /* 정리 실패 무시 */ }

            _listener = null;
            _listenerThread = null;

            Debug.Log("[DebugServer] Stopped");
        }

        #endregion

        #region Endpoint Registration

        private void RegisterAllEndpoints()
        {
            _routes.Clear();
            _endpointInfos.Clear();

            // 자기 자신의 빌트인 엔드포인트 등록
            RegisterEndpointsFrom(this);

            // 하위 IDebugEndpointProvider 컴포넌트 탐색
            var providers = GetComponentsInChildren<IDebugEndpointProvider>();
            foreach (var provider in providers)
                RegisterEndpointsFrom((MonoBehaviour)provider);

            _registeredEndpointCount = _routes.Count;
        }

        private void RegisterEndpointsFrom(MonoBehaviour target)
        {
            // Provider 레벨 메타데이터
            var provider = target as IDebugEndpointProvider;
            var routePrefix = provider?.RoutePrefix ?? "";
            var categoryName = provider?.CategoryName ?? "";

            var methods = target.GetType().GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<DebugEndpointAttribute>();
                if (attr == null) continue;

                // 메서드 시그니처 검증: void Method(RequestContext)
                var parameters = method.GetParameters();
                if (parameters.Length != 1 || parameters[0].ParameterType != typeof(RequestContext))
                {
                    Debug.LogWarning(
                        $"[DebugServer] Skipping {method.Name}: signature must be void(RequestContext)");
                    continue;
                }

                Action<RequestContext> handler;
                try
                {
                    handler = (Action<RequestContext>)method.CreateDelegate(
                        typeof(Action<RequestContext>), target);
                }
                catch
                {
                    Debug.LogWarning($"[DebugServer] Failed to bind {method.Name}");
                    continue;
                }

                // 라우트: prefix + attr.Route
                var fullRoute = routePrefix + attr.Route;
                // 카테고리: provider 레벨 우선, attr 폴백
                var category = !string.IsNullOrEmpty(categoryName) ? categoryName : attr.Category;

                var entry = new RouteEntry(attr.HttpMethod, fullRoute, handler);
                _routes.Add(entry);
                _endpointInfos.Add(new DebugEndpointInfo(attr.HttpMethod, fullRoute, attr.Description, category));
            }
        }

        #endregion

        #region Request Processing

        private void ListenLoop()
        {
            while (_isRunning && _listener != null && _listener.IsListening)
            {
                try
                {
                    var httpContext = _listener.GetContext();

                    // CORS preflight 즉시 응답 (백그라운드 스레드에서 처리 가능)
                    if (httpContext.Request.HttpMethod == "OPTIONS")
                    {
                        httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                        httpContext.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                        httpContext.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
                        httpContext.Response.StatusCode = 204;
                        httpContext.Response.Close();
                        continue;
                    }

                    var ctx = new RequestContext(httpContext);
                    _requestQueue.Enqueue(ctx);
                }
                catch (HttpListenerException)
                {
                    // 서버 종료 시 정상
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception e)
                {
                    if (_isRunning)
                        Debug.LogWarning($"[DebugServer] Listen error: {e.Message}");
                }
            }
        }

        private void ProcessRequest(RequestContext ctx)
        {
            // 라우트 매칭
            foreach (var route in _routes)
            {
                if (route.HttpMethod != ctx.HttpMethod) continue;

                if (TryMatchRoute(ctx.Path, route.Pattern, out var pathParams))
                {
                    ctx.PathParams = pathParams;
                    route.Handler(ctx);
                    return;
                }
            }

            // 매칭 실패
            ctx.RespondError($"No route matched: {ctx.HttpMethod} {ctx.Path}", 404);
        }

        private static bool TryMatchRoute(string requestPath, string routePattern,
            out Dictionary<string, string> pathParams)
        {
            pathParams = new Dictionary<string, string>();

            var reqSegments = requestPath.Trim('/').Split('/');
            var routeSegments = routePattern.Trim('/').Split('/');

            if (reqSegments.Length != routeSegments.Length) return false;

            for (var i = 0; i < routeSegments.Length; i++)
            {
                var seg = routeSegments[i];

                if (seg.StartsWith("{") && seg.EndsWith("}"))
                {
                    // 경로 파라미터 캡처
                    var paramName = seg.Substring(1, seg.Length - 2);
                    pathParams[paramName] = reqSegments[i];
                }
                else if (!string.Equals(seg, reqSegments[i], StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Built-in Endpoints

        [DebugEndpoint("GET", "/api/ping", "서버 연결 상태 확인", Category = "시스템")]
        private void Ping(RequestContext ctx)
        {
            var uptime = Time.realtimeSinceStartup - _startTime;
            ctx.Respond(
                $"{{\"port\":{port},\"uptime\":{uptime:F1},\"endpointCount\":{_routes.Count}}}");
        }

        [DebugEndpoint("GET", "/api/help", "등록된 모든 API 엔드포인트 목록 반환 (JSON)", Category = "시스템")]
        private void Help(RequestContext ctx)
        {
            var sb = new StringBuilder();
            sb.Append("[");

            for (var i = 0; i < _endpointInfos.Count; i++)
            {
                if (i > 0) sb.Append(",");
                var ep = _endpointInfos[i];
                sb.Append($"{{\"method\":\"{RequestContextExtensions.EscapeJson(ep.HttpMethod)}\"");
                sb.Append($",\"route\":\"{RequestContextExtensions.EscapeJson(ep.Route)}\"");
                sb.Append($",\"description\":\"{RequestContextExtensions.EscapeJson(ep.Description)}\"");
                sb.Append($",\"category\":\"{RequestContextExtensions.EscapeJson(ep.Category)}\"}}");
            }

            sb.Append("]");
            ctx.Respond(sb.ToString());
        }

        [DebugEndpoint("GET", "/api/help/markdown", "API 문서를 Markdown 형식으로 반환", Category = "시스템")]
        private void HelpMarkdown(RequestContext ctx)
        {
            var md = GenerateMarkdownDoc();
            ctx.Respond($"\"{RequestContextExtensions.EscapeJson(md)}\"");
        }

        #endregion

        #region Documentation Generation

        /// <summary>
        /// 등록된 엔드포인트 정보를 반환한다. Editor 스크립트에서 문서 생성에 사용할 수 있다.
        /// </summary>
        public IReadOnlyList<DebugEndpointInfo> GetEndpointInfos() => _endpointInfos;

        /// <summary>
        /// Markdown 형식의 API 문서를 생성한다.
        /// </summary>
        public string GenerateMarkdownDoc()
        {
            var sb = new StringBuilder();
            sb.AppendLine("# DebugServer API Documentation");
            sb.AppendLine();
            sb.AppendLine($"> Auto-generated at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"> Port: {port}");
            sb.AppendLine();

            // 카테고리별 그룹핑
            var grouped = _endpointInfos
                .GroupBy(e => string.IsNullOrEmpty(e.Category) ? "기타" : e.Category)
                .OrderBy(g => g.Key);

            foreach (var group in grouped)
            {
                sb.AppendLine($"## {group.Key}");
                sb.AppendLine();
                sb.AppendLine("| Method | Endpoint | Description |");
                sb.AppendLine("|--------|----------|-------------|");

                foreach (var ep in group.OrderBy(e => e.Route))
                {
                    sb.AppendLine($"| `{ep.HttpMethod}` | `{ep.Route}` | {ep.Description} |");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        #endregion

        #region Internal Types

        private sealed class RouteEntry
        {
            public readonly string HttpMethod;
            public readonly string Pattern;
            public readonly Action<RequestContext> Handler;

            public RouteEntry(string httpMethod, string pattern, Action<RequestContext> handler)
            {
                HttpMethod = httpMethod;
                Pattern = pattern;
                Handler = handler;
            }
        }

        #endregion
    }
}
#endif
