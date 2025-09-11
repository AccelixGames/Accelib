#if UNITY_EDITOR
using System;
using Accelib.EditorTool.Google.Control.Auth;
using Accelib.EditorTool.Google.Model;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Accelib.EditorTool.Google.Control.Sheets
{
    [CreateAssetMenu(fileName = "(Google) SheetDownloader-Auth", menuName = "Accelib.Google/SheetDownloader-Auth", order = 0)]
    public class GoogleSheetsDownloader_Auth : GoogleSheetsDownloaderBase
    {
        [Header("# OAuth 정보")]
        [SerializeField] private GoogleOAuthHelper oAuthHelper;
        
        private const string BaseURL = "https://sheets.googleapis.com/v4/spreadsheets";

        public override async UniTask<JSheetData> DownloadAsSheetDataAsync()
        {
            var json = await DownloadAsync();
            return JsonConvert.DeserializeObject<JSheetData>(json);
        }
        
        public override async UniTask<string> DownloadAsync()
        {
            if (string.IsNullOrEmpty(SheetId) || string.IsNullOrEmpty(Range))
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

            // URL 생성 후 요청
            var url =  $"{BaseURL}/{SheetId}/values/{Uri.EscapeDataString(Range)}";
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
    }
}
#endif