#if UNITY_EDITOR
using Accelib.EditorTool.Google.Control.Utility;
using Accelib.EditorTool.Google.Model;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Accelib.EditorTool.Google.Control.Sheets
{
    [CreateAssetMenu(fileName = "(Google) SheetDownloader-WebAPI", menuName = "Accelib.Google/SheetDownloader-WebAPI", order = 1)]
    public class GoogleSheetsDownloader_WebAPI : GoogleSheetsDownloaderBase
    {
        public enum Mode {Default, Website}
        public enum Format {Tsv, Csv}
        
        private const string BaseURL = "https://docs.google.com/spreadsheets/d";
        private const string BaseURLWeb = "https://docs.google.com/spreadsheets/d/e";

        [Header("#다운로드 모드")]
        [SerializeField] private Mode mode;
        [SerializeField] private Format format = Format.Tsv;

        public Format CurrFormat => format;
        
        public void SetFormat(Format form)
        {
            format = form;
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif
        }

        public override async UniTask<JSheetData> DownloadAsSheetDataAsync()
        {
            var raw = await DownloadAsync();
            var sheetData = new JSheetData
            {
                range = Range,
                majorDimension = "ROWS"
            };
            
            if (format == Format.Tsv)
                sheetData.values = TsvReader.Read(raw);
            else if (format == Format.Csv) 
                sheetData.values = CsvReader.Read(raw);
            
            return sheetData;
        }

        public override async UniTask<string> DownloadAsync()
        {
            if (string.IsNullOrEmpty(SheetId) || string.IsNullOrEmpty(Range))
            {
                Debug.LogError("[구글시트] sheetID 혹은 range가 비어있어, 다운로드할 수 없습니다.", this);
                return null;
            }

            var form = format == Format.Tsv ? "tsv" : "csv"; 
            var url = "";
            url = mode == Mode.Default ? 
                $"{BaseURL}/{SheetId}/export?format={form}&gid={Range}" : 
                $"{BaseURLWeb}/{SheetId}/pub?gid={Range}&single=true&output={form}";
            
            Debug.Log($"[구글시트] 데이터 다운로드 시작: {url}", this);
            
            var www = UnityWebRequest.Get(url);
            await www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("[구글시트] 다운로드 실패: " + www.error, this);
                return null;
            }
            
            Debug.Log("[구글시트] 데이터 다운로드 성공!\n" + www.downloadHandler.text, this);
            return www.downloadHandler.text;
        }
    }
}
#endif