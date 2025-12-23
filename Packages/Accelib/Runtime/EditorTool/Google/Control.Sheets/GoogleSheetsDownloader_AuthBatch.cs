using System.Collections.Generic;
using Accelib.EditorTool.Google.Control.Auth;
using Accelib.EditorTool.Google.Model;
using Accelib.EditorTool.Google.Model.Metadata;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;

namespace Accelib.EditorTool.Google.Control.Sheets
{
    [CreateAssetMenu(fileName = "(Google) SheetDownloader-AuthBatch", menuName = "Accelib.Google/SheetDownloader-AuthBatch", order = 1)]
    public class GoogleSheetsDownloader_AuthBatch : GoogleSheetsDownloaderBase
    {
        [Header("# OAuth 정보")]
        [SerializeField] private GoogleOAuthHelper oAuthHelper;
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
            var accessToken = await oAuthHelper.GetValidAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                Debug.LogError("[구글시트] AccessToken 가져오기 실패", this);
                return null;
            }
            
            // 범위 가져오기
            await GetRanges(accessToken);
            
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

        private async UniTask GetRanges(string accessToken)
        {
            // URL 생성 후 요청
            var url =  $"{BaseURL}/{SheetId}";
            Debug.Log($"[구글시트] 범위 요청: {url}", this);
            
            using var www = UnityWebRequest.Get(url);
            www.SetRequestHeader("Authorization", "Bearer " + accessToken);
            await www.SendWebRequest();
            
            var json = www.downloadHandler.text;
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[구글시트] 다운로드 실패: {www.error}\n{json}", this);
                return;
            }
            
            var metadata = JsonConvert.DeserializeObject<JSheetMetadata>(json);
            ranges.Clear();
            foreach (var sheet in metadata.sheets)
            {
                var title = sheet.properties.title;
                if(title.StartsWith('@')) continue;
                if(title.StartsWith('!')) continue;

                ranges.Add(title);
            }
        }
        
        [Button]
        private async void Test_Download()
        {
            var res = await DownloadAsSheetDataAsync();
            foreach (var resValueRange in res.valueRanges)
            {
                Debug.Log("범위: " + resValueRange.range, this);
            }
            //Debug.Log(res, this);
        }
    }
}