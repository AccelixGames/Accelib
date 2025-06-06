﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Accelib.Logging;
using Accelib.Module.API.Model;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Accelib.Module.API.Control
{
    public static class WebRequestUtil
    {
        public static string GetAuthToken(string token) => $"Bearer {token}"; 

        public static async UniTask<WebRequestResponse<T>> RequestAsync<T>(string method, string url, Dictionary<string, object> headers, string json) where T : class
        {
            try
            {
                var bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            
                // WebRequest 생성
                using var request = new UnityWebRequest(url, method);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
            
                // header 지정
                if (headers != null)
                {
                    foreach (var header in headers)
                        request.SetRequestHeader(header.Key, (string) header.Value);
                }
                
                // http 요청
                await request.SendWebRequest().ToUniTask();
                
                // http 응답 역직렬화
                
                var isDeserialize = TryDeserializeObject<T>(request.downloadHandler.text, out var jsonObj);
                if (!isDeserialize) jsonObj = null;
                
                // http 응답 객체 생성
                var isSuccess = request.result == UnityWebRequest.Result.Success && request.responseCode is >= 200 and < 300;
                var response = new WebRequestResponse<T>
                {
                    isSuccess = isSuccess,
                    status = request.responseCode,
                    message = isSuccess ? "" : request.downloadHandler.text,
                    data = isSuccess ? jsonObj : null,
                };
            
                Deb.Log($"url : {url}\n" +
                        $"request : {json}\n" +
                        $"response : {JsonConvert.SerializeObject(response, Formatting.Indented)}");

                return response;
            }
            catch (Exception e)
            {
                var response = WebRequestResponse<T>.Exception(e.Message);
                var match = Regex.Match(e.Message, @"HTTP\/1\.1 (\d{3})");
                if (match.Success)
                    long.TryParse(match.Groups[1].Value, out response.status);
                
                Deb.LogWarning($"url : {url}\n"+
                                 $"request : {json}\n" +
                                 $"response : {JsonConvert.SerializeObject(response, Formatting.Indented)}");
                return response;
            }
        }
        
        public static Dictionary<string, object> CreateDictionary(params (string key, object value)[] tuples)
        {
            var body = new Dictionary<string, object>();
            foreach (var (key, value) in tuples) 
                body.Add(key, value);

            return body;
        }

        private static bool TryDeserializeObject<T>(string json, out T result)
        {
            try
            {
                result = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch (Exception e)
            {
                result = default;
                return false;
            }
        }
    }
}