#if ACCELIB_AIT
using System;
using System.Runtime.InteropServices;
using Accelib.AccelixWeb.Model;
using Accelib.AccelixWeb.Module.Advertisement.Control;
using Accelib.AccelixWeb.Module.Advertisement.Model;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable InconsistentNaming

namespace Accelib.AccelixWeb
{
    public static class AppInTossNative
    {
        public const string Unknown = "unknown";
        
        public enum FeedbackType
        {
            basicWeak,
            basicMedium,
            tickWeak,
            tickMedium,
            softMedium,
            tap,
            success,
            error,
            wiggle,
            confetti
        }

        public static void HapticFeedback(FeedbackType type) => HandleHapticFeedback(type.ToString());
        public static async UniTask HapticFeedbackLong(FeedbackType type, float duration)
        {
            var t = type.ToString();
            var timer = 0f;
            while (timer < duration)
            {
                HandleHapticFeedback(t);
                timer += Time.unscaledDeltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }

        public static SafeAreaInsets GetSafeArea()
        {
#if UNITY_EDITOR
            var safeArea = Screen.safeArea;
            return new SafeAreaInsets((int)(Screen.height - safeArea.yMax), (int)safeArea.yMin);
#else
            try
            {
                var json = GetSafeAreaInsets();
                var obj = JsonConvert.DeserializeObject<SafeAreaInsets>(json);
                obj ??= new SafeAreaInsets();
                return obj;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new SafeAreaInsets();
            }
#endif
        }

#if UNITY_EDITOR
        public static string GetDeviceId() => "device_unity";
        public static string GetOperationalEnvironment() => "os_unity";
        public static string GetTossAppVersion() => "version_unity";
        public static string GetPlatformOS() => "unity";
        public static string GetSchemeUri() => "unity_editor";
        public static string GetLocale() => "korean";
        
        /// <summary>
        /// {link} 는 딥링크로 치환됩니다.
        /// </summary>
        public static void HandleShare(string msg, string deepLink) => Debug.Log($"[AppInTossUtility] {nameof(HandleShare)}({msg}) called");
        private static void HandleHapticFeedback(string type)
        {
            // Debug.Log($"[AppInTossUtility] {nameof(HandleHapticFeedback)}({type}) called");
        }

        public static void CallAds(string _unityCallerName, string _unitId, bool _isLoad, bool _isInterstitial) => Debug.LogError($"유니티 에디터에서 사용할 수 없는 함수입니다.");

#else
        [DllImport("__Internal")]
        public static extern string GetDeviceId();

        [DllImport("__Internal")]
        public static extern string GetOperationalEnvironment();

        [DllImport("__Internal")]
        public static extern string GetTossAppVersion();

        [DllImport("__Internal")]
        public static extern string GetPlatformOS();

        [DllImport("__Internal")]
        public static extern string GetSchemeUri();

        [DllImport("__Internal")]
        public static extern string GetLocale();

        [DllImport("__Internal")]
        public static extern void HandleShare(string msg, string deepLink);
        
        [DllImport("__Internal")]
        private static extern void HandleHapticFeedback(string type);
        
        
        [DllImport("__Internal")]
        private static extern string GetSafeAreaInsets();
        
        [DllImport("__Internal")]
        public static extern void CallAds(string _unityCallerName, string _unitId, bool _isLoad, bool _isInterstitial);
#endif
    }
}
#endif