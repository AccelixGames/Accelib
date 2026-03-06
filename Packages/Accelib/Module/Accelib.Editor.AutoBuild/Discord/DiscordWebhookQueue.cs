using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Text;
using System.Threading;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Accelib.Editor.Utility.Discord
{
    /// <summary>Discord Webhook 메시지를 큐잉하여 순차 전송한다.</summary>
    public static class DiscordWebhookQueue
    {
        private static readonly ConcurrentQueue<(string url, string json)> _queue = new();
        private static readonly HttpClient _client = new();
        private static int _isProcessing;

        private const int MaxRetries = 3;

        /// <summary>텍스트 콘텐츠와 Embed를 포함한 메시지를 큐에 추가한다.</summary>
        public static void SendMsg(string url, string content, params JDiscordEmbed[] embeds)
        {
            SendMsg(url, new JDiscordMsg { content = content, embeds = embeds });
        }

        /// <summary>JDiscordMsg 객체를 큐에 추가한다.</summary>
        public static void SendMsg(string url, JDiscordMsg msg)
        {
            var json = JsonConvert.SerializeObject(msg);
            Debug.Log(json);
            _queue.Enqueue((url, json));

            // 큐 처리가 안 돌고 있으면 시작
            if (Interlocked.CompareExchange(ref _isProcessing, 1, 0) == 0)
                System.Threading.Tasks.Task.Run(ProcessQueue);
        }

        private static void ProcessQueue()
        {
            try
            {
                while (_queue.TryDequeue(out var item))
                {
                    var sent = false;
                    for (var attempt = 0; attempt < MaxRetries && !sent; attempt++)
                    {
                        try
                        {
                            var content = new StringContent(item.json, Encoding.UTF8, "application/json");
                            var response = _client.PostAsync(item.url, content).GetAwaiter().GetResult();

                            // Rate limit (429) 처리: 지수 백오프 후 재시도
                            if ((int)response.StatusCode == 429)
                            {
                                var retryMs = 2000 * (attempt + 1);
                                Debug.LogWarning($"Discord rate limited. Retry in {retryMs}ms...");
                                Thread.Sleep(retryMs);
                                continue;
                            }

                            if (!response.IsSuccessStatusCode)
                                Debug.LogError($"Discord webhook failed: {response.StatusCode}");

                            sent = true;
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Discord webhook error (attempt {attempt + 1}): {e.Message}");
                            if (attempt < MaxRetries - 1)
                                Thread.Sleep(1000);
                        }
                    }

                    // Discord rate limit 방지 + 순서 보장을 위한 최소 딜레이
                    Thread.Sleep(200);
                }
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, 0);

                // 처리 중 새 메시지가 들어왔을 수 있으므로 재확인
                if (!_queue.IsEmpty && Interlocked.CompareExchange(ref _isProcessing, 1, 0) == 0)
                    System.Threading.Tasks.Task.Run(ProcessQueue);
            }
        }
    }
}
