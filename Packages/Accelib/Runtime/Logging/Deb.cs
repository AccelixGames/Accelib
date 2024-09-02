using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Accelib.Logging
{
    public static class Deb
    {
        private static readonly StringBuilder Builder;
        static Deb() => Builder = new StringBuilder();

        private static string ParseMessage(object message, string memberName, string sourceFilePath,
            int sourceLineNumber)
        {
            Builder.Clear();

#if !UNITY_EDITOR
            Builder.Append("[Unity]");
#endif

            if (!string.IsNullOrEmpty(sourceFilePath))
                Builder.Append(sourceFilePath.Split(@"\")[^1]?.Replace(".cs", ""));

            if (!string.IsNullOrEmpty(memberName))
                Builder.Append($"({memberName}): ");

            Builder.Append(message);

            return Builder.ToString();
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(object message, Object context = null,
            [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            Debug.Log(ParseMessage(message, memberName, sourceFilePath, sourceLineNumber), context);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogWarning(object message, Object context = null,
            [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {

            Debug.LogWarning(ParseMessage(message, memberName, sourceFilePath, sourceLineNumber), context);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogError(object message, Object context = null,
            [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            Debug.LogError(ParseMessage(message, memberName, sourceFilePath, sourceLineNumber), context);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogException(System.Exception exception, Object context = null,
            [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            Debug.LogException(exception, context);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color = default, float duration = 0.0f,
            bool depthTest = true)
        {
            Debug.DrawLine(start, end, color, duration, depthTest);
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG")]
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0.0f,
            bool depthTest = true)
        {
            Debug.DrawRay(start, dir, color, duration, depthTest);
        }
    }
}