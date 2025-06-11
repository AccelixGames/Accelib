#if ACCELIB_TELEMETRY
using Newtonsoft.Json;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Analytics;
#endif
using System.Collections.Generic;
using UnityEngine;
using Accelib.Core;
using NaughtyAttributes;

namespace Accelib.Module.Telemetry
{
    public class AnalyticsSingleton<T> : MonoSingleton<T> where T : MonoBehaviour
    {
        [SerializeField, ReadOnly] private bool isInitialized = false;
        [SerializeField, ReadOnly] private string environment;
        
        private const string ProdEnv = "production";
        private const string DevEnv = "development";
        
        private string GetEnvironment()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return DevEnv;
#else
            return ProdEnv;
#endif
        }
        
        private void Start()
        {
            Init();
        }

        private async void Init()
        {
#if ACCELIB_TELEMETRY
            
            // 개발 환경
            environment = GetEnvironment();
            
            var options = new InitializationOptions();
            options.SetEnvironmentName(environment);
            
            // 초기화
            await UnityServices.InitializeAsync(options);
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                Debug.Log("Unity Service Init 실패");
                return;
            }

            AnalyticsService.Instance.StartDataCollection();
            isInitialized = true;
#endif
        }

        /// <summary>
        /// 튜플 형식으로 커스텀 이벤트 전송
        /// </summary>
        public void RecordEvent(string eventName, params (string key, object value)[] data)
        {
#if ACCELIB_TELEMETRY
            if (!isInitialized) return;
            if (string.IsNullOrEmpty(eventName)) return;

            if(data == null)
                AnalyticsService.Instance.RecordEvent(eventName);
            else
            {
                var customEvent = new CustomEvent(eventName);
                foreach (var (key, value) in data)
                {
                    if (!string.IsNullOrEmpty(key))
                        customEvent.Add(key, value);
                }
                AnalyticsService.Instance.RecordEvent(customEvent);
                var json = JsonConvert.SerializeObject(data);
                Debug.Log($"[AnalyticsSingleton][{environment}] RecordEvent : {eventName} \n{json}");
            }
            
#endif
#if ACCELIB_TELEMETRY && UNITY_EDITOR
            AnalyticsService.Instance.Flush();
#endif
        }
        
        /// <summary>
        /// 딕셔너리 형식으로 커스텀 이벤트 전송
        /// </summary>
        public void RecordEvent(string eventName, Dictionary<string, object> data)
        {
#if ACCELIB_TELEMETRY
            if (!isInitialized) return;
            if (string.IsNullOrEmpty(eventName)) return;

            if(data == null)
                AnalyticsService.Instance.RecordEvent(eventName);
            else
            {
                var customEvent = new CustomEvent(eventName);
                foreach (var (key, value) in data)
                {
                    if (!string.IsNullOrEmpty(key))
                        customEvent.Add(key, value);
                }
                AnalyticsService.Instance.RecordEvent(customEvent);
                var json = JsonConvert.SerializeObject(data);
                Debug.Log($"[AnalyticsSingleton][{environment}] RecordEvent : {eventName} \n{json}");
            }
            
#endif
#if ACCELIB_TELEMETRY && UNITY_EDITOR
            AnalyticsService.Instance.Flush();
#endif
        }
    }
}
