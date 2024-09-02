using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Accelix.Dialogue.Editor.GoogleSheet
{
    [CreateAssetMenu(fileName = "googleSheetDownloader", menuName = "Accelix-GoogleSheetDownloader", order = 0)]
    public class GoogleSheetDownloader : ScriptableObject
    {
        private enum Mode {Default, Website}
        private enum Format {Tsv, Csv}
        
        private const string BaseURL = "https://docs.google.com/spreadsheets/d";
        private const string BaseURLWeb = "https://docs.google.com/spreadsheets/d/e";

        [SerializeField] private Mode mode;
        [SerializeField] private Format format = Format.Tsv;
        [SerializeField, TextArea] private string key = "";
        [SerializeField, TextArea] private string gid;
        
        //[SerializeField, TextArea] private string baseURL = "https://docs.google.com/spreadsheets/d/1DMSKxCm5JsSehRV5VSFljTrnYKmvo3TnsXspkkEuD4w/export?format=tsv&gid=496468716";

        // https://docs.google.com/spreadsheets/d/e/2PACX-1vR2kg34U9u0peCjq7UsCs4ZVSv2o17CdQgEqp-Rwye4nwBlpIhwhTEZbRtmNJfp8ZkKHDWszNWGb2FP/pub?gid=496468716&single=true&output=tsv
        // https://docs.google.com/spreadsheets/d/e/2PACX-1vR2kg34U9u0peCjq7UsCs4ZVSv2o17CdQgEqp-Rwye4nwBlpIhwhTEZbRtmNJfp8ZkKHDWszNWGb2FP/pub?gid=1806491859&single=true&output=tsv
        // https://docs.google.com/spreadsheets/d/e/2PACX-1vR2kg34U9u0peCjq7UsCs4ZVSv2o17CdQgEqp-Rwye4nwBlpIhwhTEZbRtmNJfp8ZkKHDWszNWGb2FP/pub?gid=496468716&single=true&output=tsv
        // https://docs.google.com/spreadsheets/d/e/2PACX-1vR2kg34U9u0peCjq7UsCs4ZVSv2o17CdQgEqp-Rwye4nwBlpIhwhTEZbRtmNJfp8ZkKHDWszNWGb2FP/pub?gid=496468716/single=true&output=tsv
        // https://docs.google.com/spreadsheets/d/e/2PACX-1vR2kg34U9u0peCjq7UsCs4ZVSv2o17CdQgEqp-Rwye4nwBlpIhwhTEZbRtmNJfp8ZkKHDWszNWGb2FP/pub?gid=496468716&single=true&output=tsv
        
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
            
            //Debug.Log("[구글시트] 데이터 다운로드 성공!\n" + www.downloadHandler.text, this);
            return www.downloadHandler.text;
        }
    }
}