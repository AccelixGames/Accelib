#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Accelib.DebugServer
{
    /// <summary>
    /// HTTP 요청/응답을 캡슐화하는 컨텍스트. 메인 스레드에서 처리된다.
    /// </summary>
    public sealed class RequestContext
    {
        private readonly HttpListenerContext _httpContext;

        public string HttpMethod { get; }
        public string Path { get; }
        public string Body { get; }
        public Dictionary<string, string> PathParams { get; internal set; }

        internal RequestContext(HttpListenerContext httpContext)
        {
            _httpContext = httpContext;
            HttpMethod = httpContext.Request.HttpMethod.ToUpperInvariant();
            Path = httpContext.Request.Url?.AbsolutePath ?? "/";
            PathParams = new Dictionary<string, string>();

            // POST body 읽기
            if (httpContext.Request.HasEntityBody)
            {
                using var reader = new StreamReader(httpContext.Request.InputStream, Encoding.UTF8);
                Body = reader.ReadToEnd();
            }
            else
            {
                Body = string.Empty;
            }
        }

        /// <summary>
        /// 경로 파라미터를 가져온다. 없으면 defaultValue를 반환한다.
        /// </summary>
        public string GetPathParam(string name, string defaultValue = null)
        {
            return PathParams != null && PathParams.TryGetValue(name, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// 경로 파라미터를 int로 가져온다. 실패 시 defaultValue를 반환한다.
        /// </summary>
        public int GetPathParamInt(string name, int defaultValue = -1)
        {
            var str = GetPathParam(name);
            return int.TryParse(str, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// JSON 응답을 전송한다.
        /// </summary>
        public void SendResponse(string json, int statusCode = 200)
        {
            var response = _httpContext.Response;
            response.StatusCode = statusCode;
            response.ContentType = "application/json; charset=utf-8";
            response.Headers.Add("Access-Control-Allow-Origin", "*");

            var buffer = Encoding.UTF8.GetBytes(json);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }
}
#endif
