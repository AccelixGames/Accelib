#if UNITY_EDITOR
using Accelib.EditorTool.Google.Model;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Accelib.EditorTool.Google.Control.Sheets
{
    public abstract class GoogleSheetsDownloaderBase : ScriptableObject
    {
        [Header("#시트 정보")]
        [FormerlySerializedAs("key")][SerializeField, TextArea] private string sheetId = "";
        [FormerlySerializedAs("gid")][SerializeField, TextArea] private string range = "SheetName!A-Z";

        public string SheetId => sheetId;
        public string Range => range;

        public abstract UniTask<JSheetData> DownloadAsSheetDataAsync();
        
        public abstract UniTask<string> DownloadAsync();
        
        [Button("다운로드(테스트)", EButtonEnableMode.Editor)]
        private void Download() => DownloadAsync().Forget();
    }
}
#endif