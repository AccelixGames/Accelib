using Cysharp.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Accelib.Editor.Utility.Discord
{
    public static class DiscordWebhook
    {
        public static void SendMsg(string url, string content, params JDiscordEmbed[] embeds)
        {
            SendMsgAsync(url, new JDiscordMsg
            {
                content = content,
                embeds = embeds
            }).Forget();
        }

        public static void SendMsg(string url, JDiscordMsg msg) => SendMsgAsync(url, msg).Forget();

        private static async UniTask<bool> SendMsgAsync(string url, JDiscordMsg msg)
        {
            var json = JsonConvert.SerializeObject(msg);
            Debug.Log(json);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            
            // UnityWebRequest를 사용해 POST 요청 생성
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 요청 보내기
            await request.SendWebRequest();

            // 실패 처리
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                return false;
            }
            
            return true;
        }
    }
}