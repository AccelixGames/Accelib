#if UNITY_EDITOR
using Accelib.EditorTool.Google.Control.Auth;
using Accelib.EditorTool.Google.Model;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Accelib.EditorTool.Google.Control.Sheets
{
    [CreateAssetMenu(fileName = "(Google) SheetDownloader-Url", menuName = "Accelib.Google/SheetDownloader-Url", order = 2)]
    public class GoogleSheetsDownloader_SimpleUrlAuth : GoogleSheetsDownloaderBase
    {
        [Header("# OAuth 정보")]
        [SerializeField] private GoogleOAuthHelper oAuthHelper;
        
        [Header("URL")]
        [TextArea, SerializeField] private string url;
        
        public override async UniTask<JSheet> DownloadAsSheetDataAsync()
        {
            var json = await DownloadAsync();
            return JsonConvert.DeserializeObject<JSheet>(json);
        }

        public override async UniTask<string> DownloadAsync()
        {
            // AccessToken 가져오기
            var accessToken = await oAuthHelper.GetValidAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                Debug.LogError("[구글시트] AccessToken 가져오기 실패", this);
                return null;
            }
            
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