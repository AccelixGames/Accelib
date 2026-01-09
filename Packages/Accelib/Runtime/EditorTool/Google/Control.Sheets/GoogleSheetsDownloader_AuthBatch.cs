#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Accelib.EditorTool.Google.Control.Auth;
using Accelib.EditorTool.Google.Model;
using Accelib.EditorTool.Google.Model.Metadata;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Accelib.EditorTool.Google.Control.Sheets
{
    [CreateAssetMenu(fileName = "(Google) SheetDownloader-AuthBatch", menuName = "Accelib.Google/SheetDownloader-AuthBatch", order = 1)]
    public class GoogleSheetsDownloader_AuthBatch : GoogleSheetsDownloaderBase
    {
        [Header("# OAuth 정보")]
        [SerializeField] private List<string> ranges;
        
        private const string BaseURL = "https://sheets.googleapis.com/v4/spreadsheets";
        
        public override async UniTask<JSheet> DownloadAsSheetDataAsync()
        {
            var json = await DownloadAsync();
            return JsonConvert.DeserializeObject<JSheet>(json);
        }

        public override async UniTask<string> DownloadAsync()
        {
            if (string.IsNullOrEmpty(SheetId))
            {
                Debug.LogError("[구글시트] SheetId 혹은 Range가 비어있어, 다운로드할 수 없습니다.", this);
                return null;
            }
            
            // AccessToken 가져오기
            var accessToken = await GoogleOAuthHelper.GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                Debug.LogError("[구글시트] AccessToken 가져오기 실패", this);
                return null;
            }
            
            // 범위 가져오기
            var rangeUrl =  $"{BaseURL}/{SheetId}";
            ranges.Clear();
            ranges = await GetRanges(rangeUrl, accessToken, this);
            
            // URL 생성 후 요청
            var url =  $"{BaseURL}/{SheetId}/values:batchGet?";
            foreach (var range in ranges) url += $"ranges={range}&";
            Debug.Log($"[구글시트] 데이터 다운로드 시작: {url}", this);
            
            using var www = UnityWebRequest.Get(url);
            www.SetRequestHeader("Authorization", "Bearer " + accessToken);
            await www.SendWebRequest();

            var json = www.downloadHandler.text;
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[구글시트] 다운로드 실패: {www.error}\n{json}", this);
                return null;
            }

            Debug.Log("[구글시트] 데이터 다운로드 성공!\n" + json, this);
            return json;
        }

        public static async UniTask<List<string>> GetRanges(string url, string accessToken, Object ctx = null)
        {
            // URL 생성 후 요청
            Debug.Log($"[구글시트] 범위 요청: {url}", ctx);
            
            using var www = UnityWebRequest.Get(url);
            www.SetRequestHeader("Authorization", "Bearer " + accessToken);
            await www.SendWebRequest();
            
            var json = www.downloadHandler.text;
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[구글시트] 다운로드 실패: {www.error}\n{json}", ctx);
                return null;
            }
            
            var metadata = JsonConvert.DeserializeObject<JSheetMetadata>(json);
            var result = new List<string>();
            foreach (var sheet in metadata.sheets)
            {
                var title = sheet.properties.title;
                if(title.StartsWith('@')) continue;
                if(title.StartsWith('!')) continue;

                result.Add(title);
            }

            return result;
        }

        public static async UniTask<JSheet> DownloadAsSheetDataAsyncStatic(string sheetId, List<string> ranges, string accessToken, Object ctx = null)
        {
            if (string.IsNullOrEmpty(sheetId))
                throw new NullReferenceException("[구글시트] SheetId 혹은 Range가 비어있어, 다운로드할 수 없습니다.");
            
            // AccessToken 가져오기
            if (string.IsNullOrEmpty(accessToken))
                throw new NullReferenceException("[구글시트] AccessToken 가져오기 실패");
            
            // URL 생성 후 요청
            var url =  $"{BaseURL}/{sheetId}/values:batchGet?";
            foreach (var range in ranges) url += $"ranges={range}&";
            Debug.Log($"[구글시트] 데이터 다운로드 시작: {url}", ctx);
            
            using var www = UnityWebRequest.Get(url);
            www.SetRequestHeader("Authorization", "Bearer " + accessToken);
            await www.SendWebRequest();

            var json = www.downloadHandler.text;
            if (www.result != UnityWebRequest.Result.Success)
                throw new NullReferenceException("[구글시트] 다운로드 실패: {www.error}\n{json}");

            //Debug.Log("[구글시트] 데이터 다운로드 성공!\n" + json, ctx);
            return JsonConvert.DeserializeObject<JSheet>(json);
        }
    }
}
#endif