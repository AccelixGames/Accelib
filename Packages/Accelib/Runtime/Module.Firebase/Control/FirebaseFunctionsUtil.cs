#if ACCELIB_FB
using System;
using System.Collections.Generic;
using System.Text;
using Accelib.Logging;
using Accelib.Module.Firebase.Model;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Accelib.Module.Firebase.Control
{
    public static class FirebaseFunctionsUtil
    {
        public static Dictionary<string, object> CreateBody(string key, object value)
        {
            var body = new Dictionary<string, object> { { key, value } };

            return body;
        }
        
        public static Dictionary<string, object> CreateBody(params (string key, object value)[] tuple)
        {
            var body = new Dictionary<string, object>();
            foreach (var (key, value) in tuple) 
                body.Add(key, value);

            return body;
        }
        
        public static async UniTask<FirebaseResponse> RequestAsync(string url, object body = null, Object ctx = null)
        {
            try
            {
                var resultJson = await RequestAsyncInternal(url, body, ctx);
                if(string.IsNullOrEmpty(resultJson)) throw new Exception("Request failed");
                
                var parsed = JsonConvert.DeserializeObject<FirebaseResponse>(resultJson);
                return parsed;
            }
            catch (Exception e)
            {
                Deb.LogException(e, ctx);
                return FirebaseResponse.Exception(e.Message);
            }
        }
        
        public static async UniTask<FirebaseResponseT<T>> RequestAsync<T>(string url, object body = null, Object ctx = null) where T : class
        {
            try
            {
                var resultJson = await RequestAsyncInternal(url, body, ctx);
                if(string.IsNullOrEmpty(resultJson)) throw new Exception("Request failed");
                
                var parsed = JsonConvert.DeserializeObject<FirebaseResponseT<T>>(resultJson);
                return parsed;
            }
            catch (Exception e)
            {
                Deb.LogException(e, ctx);
                return FirebaseResponseT<T>.Exception(e.Message);
            }
        }
        
        private static async UniTask<string> RequestAsyncInternal(string url, object body = null, Object ctx = null)
        {
            // 리퀘스트 생성
            const string method = UnityWebRequest.kHttpVerbPOST;
            using var req = new UnityWebRequest(url, method);

            // 바디 설정
            if (body != null)
            {
                var bodyJson = JsonConvert.SerializeObject(body);
                var bodyRaw = Encoding.UTF8.GetBytes(bodyJson);
                req.uploadHandler = new UploadHandlerRaw(bodyRaw);
                req.SetRequestHeader("Content-Type", "application/json");
            }

            // 리스폰스 설정
            req.downloadHandler = new DownloadHandlerBuffer();

            // 전송
            await req.SendWebRequest();

            // 실패시 에러 반환
            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception(req.error);

            // 결과 반환
            return req.downloadHandler.text;
        }
    }
}
#endif