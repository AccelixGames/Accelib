#if ACCELIB_TELEMETRY
using Sentry;
#endif
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Accelib.Module.Telemetry
{
    public static class SentryLogger
    {
        private enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error
        }
        
        public static void LogError(string context, string message, Exception exception = null, Dictionary<string, object> contexts = null, Dictionary<string, object> extras = null)
            => Log(LogLevel.Error, context, message, exception, contexts, extras);

        public static void LogWarning(string context, string message, Dictionary<string, object> contexts = null, Dictionary<string, object> extras = null)
            => Log(LogLevel.Warning, context, message, null, contexts, extras);

        public static void LogInfo(string context, string message, Dictionary<string, object> contexts = null, Dictionary<string, object> extras = null)
            => Log(LogLevel.Info, context, message, null, contexts, extras);

        public static void LogDebug(string context, string message, Dictionary<string, object> contexts = null, Dictionary<string, object> extras = null)
            => Log(LogLevel.Debug, context, message, null, contexts, extras);

        private static void Log(LogLevel level, string context, string message, Exception exception = null, Dictionary<string, object> contexts = null, Dictionary<string, object> extras = null)
        {
            var formatted = $"[{level}] [{context}] {message}";
            Debug.Log($"Sentry Log : {formatted}");

#if ACCELIB_TELEMETRY
            var sentryLevel = ToSentryLevel(level);

            SentrySdk.ConfigureScope(scope =>
            {
                scope.Level = sentryLevel;
                scope.SetTag("context", context);

                if(contexts != null)
                    scope.Contexts["Custom Data"] = contexts;

                if (extras != null)
                {
                    foreach (var kv in extras)
                        scope.SetExtra(kv.Key, kv.Value);
                }

                if (exception != null)
                    SentrySdk.CaptureException(exception);
                else
                    SentrySdk.CaptureMessage(formatted, sentryLevel);
            });
#endif
        }

#if ACCELIB_TELEMETRY
        private static SentryLevel ToSentryLevel(LogLevel level)
        {
            return level switch
            {
                LogLevel.Debug => SentryLevel.Debug,
                LogLevel.Info => SentryLevel.Info,
                LogLevel.Warning => SentryLevel.Warning,
                LogLevel.Error => SentryLevel.Error,
                _ => SentryLevel.Info,
            };
        }
#endif

        private static Dictionary<string, object> ToDictionary(object obj)
        {
            if (obj == null)
                return null;

            if (obj is Dictionary<string, object> dict)
                return dict;

            var type = obj.GetType();

            if (type.IsPrimitive || obj is string || obj is decimal)
                return new Dictionary<string, object> { { "value", obj } };

            var result = new Dictionary<string, object>();
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                result[field.Name] = field.GetValue(obj);

            return result;
        }


    }
}
