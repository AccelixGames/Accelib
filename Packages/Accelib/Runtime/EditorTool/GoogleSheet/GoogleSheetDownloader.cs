using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Accelib.EditorTool.GoogleSheet
{
    [CreateAssetMenu(fileName = "googleSheetDownloader", menuName = "Accelib.Editor/GoogleSheetDownloader", order = 0)]
    public class GoogleSheetDownloader : ScriptableObject
    {
        public enum Mode {Default, Website}
        public enum Format {Tsv, Csv}
        
        private const string BaseURL = "https://docs.google.com/spreadsheets/d";
        private const string BaseURLWeb = "https://docs.google.com/spreadsheets/d/e";

        [SerializeField] private Mode mode;
        [SerializeField] private Format format = Format.Tsv;
        [SerializeField, TextArea] private string key = "";
        [SerializeField, TextArea] private string gid;

        public Format CurrFormat => format;
        
        public void SetFormat(Format form)
        {
            format = form;
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
#endif
        }
        
        public async UniTask<string> DownloadAsync()
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(gid))
            {
                Debug.LogError("[구글시트] key 혹은 gid가 비어있어, 다운로드할 수 없습니다.", this);
                return null;
            }

            var form = format == Format.Tsv ? "tsv" : "csv"; 
            var url = "";
            url = mode == Mode.Default ? 
                $"{BaseURL}/{key}/export?format={form}&gid={gid}" : 
                $"{BaseURLWeb}/{key}/pub?gid={gid}&single=true&output={form}";
            
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