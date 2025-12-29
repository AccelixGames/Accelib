using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.EditorTool.Google.Control.Auth;
using Accelib.EditorTool.Google.Control.Drive.Model;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Networking;

namespace Accelib.EditorTool.Google.Control.Drive
{
    [CreateAssetMenu(fileName = "(Google) DriveDownloader", menuName = "Accelib.Google/DriveDownloader", order = 0)]
    public class GoogleDriveDownloader : SerializedScriptableObject
    {
        [SerializeField] private GoogleOAuthHelper oAuthHelper;
        [SerializeField] private string rootFolderId;

        [Header("Result")] 
        [OdinSerialize] private GoogleDriveEntry_Folder rootFolder;

        private const int MaxPageIteration = 5;
        private const int MaxRecursiveDepth = 3;
        private const string DriveBaseURL = "https://www.googleapis.com/drive/v3/files?";
        private const string SheetBaseURL = "https://sheets.googleapis.com/v4/spreadsheets";

        [Button]
        public async UniTask LoadAsync()
        {
            var accessToken = await oAuthHelper.GetValidAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                Debug.LogError("[구글] AccessToken 가져오기 실패", this);
                return;
            }

            Debug.Log("---[구글드라이브] Sync 시작!---");
            
            rootFolder = new GoogleDriveEntry_Folder(rootFolderId, "root");
            await RecursiveQueryAsync(accessToken, rootFolder, 0);
            
            Debug.Log("---[구글드라이브] Sync 종료!---");
        }

        private async UniTask RecursiveQueryAsync(string accessToken, GoogleDriveEntry_Folder parent, int depth)
        {
            // 깊이가 너무 깊어졌다면, 종료
            if (depth >= MaxRecursiveDepth)
                return;

            // 현재 깊이의 폴더 탐색
            parent.children = await ReadFolderAsync(accessToken, parent.id);
            parent.children.Sort((a,b) => string.Compare(a.MimeType, b.MimeType, StringComparison.Ordinal));
            
            // 자식을 순회하며 탐색
            var taskList = new List<UniTask>();
            foreach (var child in parent.children)
            {
                if (child is GoogleDriveEntry_SpreadSheet childSpreadSheet)
                    taskList.Add(ReadSpreadSheetAsync(accessToken, childSpreadSheet));
                else if (child is GoogleDriveEntry_Folder childFolder) 
                    taskList.Add(RecursiveQueryAsync(accessToken, childFolder, depth + 1));
            }
            
            await UniTask.WhenAll(taskList);
        }

        private async UniTask ReadSpreadSheetAsync(string accessToken, GoogleDriveEntry_SpreadSheet spreadSheet)
        {
            try
            {
                var query = "spreadsheetId,properties.title,sheets(properties(title,sheetId))";
                var fields = Uri.EscapeDataString(query);
                var url = $"{SheetBaseURL}/{spreadSheet.id}?fields={fields}";
                Debug.Log($"[구글시트] GET: {url}", this);
                
                using var www = UnityWebRequest.Get(url);
                www.SetRequestHeader("Authorization", "Bearer " + accessToken);
                www.timeout = 5;
                await www.SendWebRequest();
                
                var json = www.downloadHandler.text;
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[구글시트] 다운로드 실패: {www.error}\n{json}", this);
                    return;
                }
                
                var data = JsonConvert.DeserializeObject<GoogleSheetResponse>(json);
                spreadSheet.sheets = new List<GoogleSheetMetadata>();
                foreach (var sheetTab in data.Sheets)
                {
                    if (sheetTab.Properties.Title.Contains('!')) continue;
                    
                    spreadSheet.sheets.Add(new GoogleSheetMetadata(spreadSheet.id, sheetTab.Properties.SheetId.ToString(), sheetTab.Properties.Title));
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private async UniTask<List<GoogleDriveEntry>> ReadFolderAsync(string accessToken, string folder)
        {
            try
            {
                var query = $"'{folder}' in parents and " +
                            $"(mimeType = '{MimeTypeStr.Folder}' or mimeType = '{MimeTypeStr.Spreadsheet}') and " +
                            "trashed = false";
                var encodedQuery = UnityWebRequest.EscapeURL(query);
                var result = new List<GoogleDriveEntry>();
                string nextPageToken = null;
                var iter = 0;

                do
                {
                    var url = DriveBaseURL +
                              $"corpora=allDrives&" +
                              // $"driveId={driveId}&" +
                              $"includeItemsFromAllDrives=true&supportsAllDrives=true&" +
                              $"pageSize=1000&" +
                              $"q={encodedQuery}&" +
                              $"fields=nextPageToken,files(id,name,mimeType)";

                    if (!string.IsNullOrEmpty(nextPageToken))
                        url += $"&pageToken={nextPageToken}";

                    Debug.Log("[구글드라이브] GET: " + url);
                    using var www = UnityWebRequest.Get(url);
                    www.SetRequestHeader("Authorization", "Bearer " + accessToken);
                    www.timeout = 5;
                    await www.SendWebRequest();

                    var lastJson = www.downloadHandler.text;
                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"[구글드라이브] 다운로드 실패: {www.error}\n{lastJson}");
                        return result;
                    }
                    else
                    {
                        Debug.Log($"[구글드라이브] 다운로드 성공: " + lastJson);
                    }

                    var jsonParsed = JsonConvert.DeserializeObject<GoogleDriveEntryList>(lastJson);
                    if (jsonParsed?.files != null)
                        foreach (var raw in jsonParsed.files)
                        {
                            if(raw.name.Contains('!')) continue;
                            
                            if(raw.mimeType == MimeTypeStr.Spreadsheet)
                                result.Add(new GoogleDriveEntry_SpreadSheet(raw.id, raw.name));
                            else if (raw.mimeType == MimeTypeStr.Folder)
                                result.Add(new GoogleDriveEntry_Folder(raw.id, raw.name));
                        }

                    nextPageToken = jsonParsed?.nextPageToken;

                    if (++iter >= MaxPageIteration)
                        break;
                } while (!string.IsNullOrEmpty(nextPageToken));

                return result;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        [Serializable]
        private class GoogleDriveEntryList
        {
            public List<GoogleDriveEntryRaw> files;
            public string nextPageToken;
        }

        [Serializable]
        private class GoogleDriveEntryRaw
        {
            public string id;
            public string name;
            public string mimeType;
        }
        
        
        [Serializable]
        public class GoogleSheetResponse
        {
            [JsonProperty("spreadsheetId")]
            public string SpreadsheetId { get; set; }

            [JsonProperty("properties")]
            public GoogleSheetProperties Properties { get; set; }

            [JsonProperty("sheets")]
            public List<GoogleSheetTab> Sheets { get; set; }
        }

        [Serializable]
        public class GoogleSheetProperties
        {
            [JsonProperty("title")]
            public string Title { get; set; }
        }

        [Serializable]
        public class GoogleSheetTab
        {
            [JsonProperty("properties")]
            public GoogleSheetTabProperties Properties { get; set; }
        }

        [Serializable]
        public class GoogleSheetTabProperties
        {
            [JsonProperty("sheetId")]
            public int SheetId { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }
        }
    }
}