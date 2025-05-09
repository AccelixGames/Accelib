#if ACCELIB_AIT
using System;
using System.Runtime.InteropServices;
using Accelib.AccelixWeb.Model;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Accelib.AccelixWeb
{
    public static class AppInTossNative
    {
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
        public static void HandleShare(string msg) => Debug.Log($"[AppInTossUtility] {nameof(HandleShare)}({msg}) called");
        private static void HandleHapticFeedback(string type) => Debug.Log($"[AppInTossUtility] {nameof(HandleHapticFeedback)}({type}) called");
        public static string GetTossAppVersion() => "unity_editor";

        public static void CallAds(string _unityCallerName, string _unitId, bool _isLoad, bool _isInterstitial)
        {
            var load = _isLoad ? "Load" : "Show";
            var inter = _isInterstitial ? "Interstitial" : "Rewarded";
            var methodName = "On" + load + inter;
            Debug.Log($"[{_unityCallerName}] {methodName} ({_unitId})");
            
            var unityCaller = GameObject.Find(_unityCallerName);
            if (unityCaller != null)
            {
                if (_isLoad)
                    unityCaller.SendMessage(methodName, "received");
                else
                    unityCaller.SendMessage(methodName, "requested");
            }
        }
        
#else
        [DllImport("__Internal")]
        public static extern void HandleShare(string msg);
        
        [DllImport("__Internal")]
        private static extern void HandleHapticFeedback(string type);
        
        [DllImport("__Internal")]
        public static extern string GetTossAppVersion();
        
        [DllImport("__Internal")]
        private static extern string GetSafeAreaInsets();
        
        [DllImport("__Internal")]
        public static extern void CallAds(string _unityCallerName, string _unitId, bool _isLoad, bool _isInterstitial);
#endif
    }
}
#endif