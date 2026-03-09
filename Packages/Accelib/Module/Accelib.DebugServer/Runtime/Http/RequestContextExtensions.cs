#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Accelib.DebugServer
{
    /// <summary>
    /// RequestContext 확장 메서드. 응답 전송, POST body 파싱 등 엔드포인트 구현에 필요한 유틸리티를 제공한다.
    /// </summary>
    public static class RequestContextExtensions
    {
        #region 응답

        /// <summary>
        /// JSON 문자열로 성공 응답을 전송한다. dataJson은 이미 직렬화된 JSON이어야 한다.
        /// </summary>
        public static void Respond(this RequestContext ctx, string dataJson)
        {
            var json = $"{{\"ok\":true,\"data\":{dataJson}}}";
            ctx.SendResponse(json);
        }

        /// <summary>
        /// 데이터 없이 성공 응답을 전송한다.
        /// </summary>
        public static void RespondOk(this RequestContext ctx)
        {
            ctx.SendResponse("{\"ok\":true}");
        }

        /// <summary>
        /// 에러 응답을 전송한다.
        /// </summary>
        public static void RespondError(this RequestContext ctx, string error, int statusCode = 400)
        {
            var json = $"{{\"ok\":false,\"error\":\"{EscapeJson(error)}\"}}";
            ctx.SendResponse(json, statusCode);
        }

        /// <summary>
        /// [Serializable] 구조체를 JSON으로 변환하여 성공 응답을 전송한다.
        /// </summary>
        public static void RespondDto<T>(this RequestContext ctx, T dto) where T : struct
        {
            ctx.Respond(JsonUtility.ToJson(dto));
        }

        /// <summary>
        /// [Serializable] 구조체 배열을 JSON 배열로 변환하여 성공 응답을 전송한다.
        /// </summary>
        public static void RespondArray<T>(this RequestContext ctx, T[] items) where T : struct
        {
            var sb = new StringBuilder("[");
            for (var i = 0; i < items.Length; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(JsonUtility.ToJson(items[i]));
            }
            sb.Append("]");
            ctx.Respond(sb.ToString());
        }

        #endregion

        #region POST Body 파싱

        /// <summary>
        /// POST body에서 int 필드를 파싱한다. 실패 시 defaultValue 반환.
        /// </summary>
        public static int ParseBodyInt(this RequestContext ctx, string fieldName, int defaultValue = 0)
        {
            var body = ctx.Body;
            if (string.IsNullOrEmpty(body)) return defaultValue;

            var key = $"\"{fieldName}\"";
            var idx = body.IndexOf(key, StringComparison.Ordinal);
            if (idx < 0) return defaultValue;

            idx += key.Length;
            while (idx < body.Length && (body[idx] == ':' || body[idx] == ' '))
                idx++;

            var numStart = idx;
            if (idx < body.Length && body[idx] == '-')
                idx++;

            while (idx < body.Length && char.IsDigit(body[idx]))
                idx++;

            var numStr = body.Substring(numStart, idx - numStart);
            return int.TryParse(numStr, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// POST body에서 float 필드를 파싱한다. 실패 시 defaultValue 반환.
        /// </summary>
        public static float ParseBodyFloat(this RequestContext ctx, string fieldName,
            float defaultValue = 0f)
        {
            var body = ctx.Body;
            if (string.IsNullOrEmpty(body)) return defaultValue;

            var key = $"\"{fieldName}\"";
            var idx = body.IndexOf(key, StringComparison.Ordinal);
            if (idx < 0) return defaultValue;

            idx += key.Length;
            while (idx < body.Length && (body[idx] == ':' || body[idx] == ' '))
                idx++;

            var numStart = idx;
            while (idx < body.Length && (char.IsDigit(body[idx]) || body[idx] == '.' || body[idx] == '-'))
                idx++;

            var numStr = body.Substring(numStart, idx - numStart);
            return float.TryParse(numStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
                ? result
                : defaultValue;
        }

        /// <summary>
        /// POST body에서 bool 필드를 파싱한다. 실패 시 defaultValue 반환.
        /// </summary>
        public static bool ParseBodyBool(this RequestContext ctx, string fieldName,
            bool defaultValue = false)
        {
            var body = ctx.Body;
            if (string.IsNullOrEmpty(body)) return defaultValue;

            var key = $"\"{fieldName}\"";
            var idx = body.IndexOf(key, StringComparison.Ordinal);
            if (idx < 0) return defaultValue;

            idx += key.Length;
            while (idx < body.Length && (body[idx] == ':' || body[idx] == ' '))
                idx++;

            if (idx + 4 <= body.Length && body.Substring(idx, 4) == "true")
                return true;
            if (idx + 5 <= body.Length && body.Substring(idx, 5) == "false")
                return false;

            return defaultValue;
        }

        #endregion

        #region 유틸리티

        /// <summary>
        /// JSON 문자열 내에서 안전하게 사용할 수 있도록 특수문자를 이스케이프한다.
        /// </summary>
        public static string EscapeJson(string s)
        {
            if (s == null) return string.Empty;
            return s
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }

        #endregion
    }
}
#endif
