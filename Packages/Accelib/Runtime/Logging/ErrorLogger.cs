using Sentry;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Accelib.Logging
{
    public static class ErrorLogger
    {
        public static void LogError(
            string context, string message,
            Exception exception = null,
            object contexts = null,
            Dictionary<string, object> extras = null)
        {
            LogWithSentry(SentryLevel.Error, context, message, exception, contexts, extras);
        }

        public static void LogWarning(
            string context,
            string message,
            object contexts = null,
            Dictionary<string, object> extras = null)
        {
            LogWithSentry(SentryLevel.Warning, context, message, null, contexts, extras);
        }

        public static void LogInfo(
            string context,
            string message,
            object contexts = null,
            Dictionary<string, object> extras = null)
        {
            LogWithSentry(SentryLevel.Info, context, message, null, contexts, extras);
        }

        public static void LogDebug(
            string context,
            string message,
            object contexts = null,
            Dictionary<string, object> extras = null)
        {
            LogWithSentry(SentryLevel.Debug, context, message, null, contexts, extras);
        }
        
        private static void LogWithSentry(SentryLevel level, string context, string message, Exception exception = null, object contexts = null, Dictionary<string, object> extras = null)
        {
            var formatted = $"[{level}] [{context}] {message}";
#if UNITY_EDITOR
            // Unity 로그 출력
            Debug.LogWarning($"Sentry Log : {formatted}");
#endif
            // Sentry 전송
            SentrySdk.ConfigureScope(scope =>
            {
                // Level
                scope.Level = level;
                // Tag
                scope.SetTag("context", context);
                // Contexts
                var dic = ToDictionary(contexts);
                if (dic != null)
                {
                    scope.Contexts["Custom Data"] = dic;
                }
                // Extras
                if (extras != null)
                {
                    foreach (var kv in extras)
                        scope.SetExtra(kv.Key, kv.Value);
                }
                // Exception or Message
                if (exception != null)
                    SentrySdk.CaptureException(exception);
                else
                    SentrySdk.CaptureMessage(formatted, level);
            });
        }
        
        public static Dictionary<string, object> ToDictionary(object obj)
        {
            if (obj == null)
                return null;

            // 이미 Dictionary<string, object>인 경우 그대로 반환
            if (obj is Dictionary<string, object> dict)
                return dict;

            var type = obj.GetType();

            // 단일 값(기본형)이면 "value" 키로 감싸기
            if (type.IsPrimitive || obj is string || obj is decimal)
            {
                return new Dictionary<string, object> { { "value", obj } };
            }

            var result = new Dictionary<string, object>();
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = field.GetValue(obj);
                result[field.Name] = value;
            }

            return result;
        }
    }
}