using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Logging;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using Accelib.EditorTool.Google.Control.Sheets;
using Accelib.EditorTool.Google.Control.Utility;
#endif

namespace Accelib.Module.AccelNovel.Model.SO
{
    [CreateAssetMenu(fileName = "scn_", menuName = "Accelib/AccelNovel/Scenario", order = 10)]
    public partial class SO_Scenario : ScriptableObject
    {
        [field: Header("시나리오 정보")] 
        [field: SerializeField] public string ScnKey { get; private set; }
        [field: SerializeField] public string ScnName { get; private set; }
        [field: SerializeField] public bool ShowNotice { get; private set; }
        // [field: SerializeField] public Sprite Thumbnail { get; private set; }

        [Header("스크립트")]
        [SerializeField] private List<ScriptLine> scriptLines;
        public IReadOnlyList<ScriptLine> ScriptLines => scriptLines;
        
        [Header("Maid 스크립트")]
        [SerializeField] private List<TempDialogueSO> dialogues;
        public IReadOnlyList<TempDialogueSO> Dialogues => dialogues;
    }

#if UNITY_EDITOR
    public partial class SO_Scenario
    {
        [Header("MetaData")]
        [SerializeField] private GoogleSheetsDownloader_WebAPI sheetDownloader;

        public async UniTask LoadGoogleSheetTask(float? customProgress = null)
        {
            var title = $"로딩중({ScnKey})..";
            var progress = 0f;
                
            // Download
            progress = customProgress ?? 0f;
            EditorUtility.DisplayProgressBar(title, $"데이터 다운로드중..({progress:P0})", progress);
            sheetDownloader.SetFormat(GoogleSheetsDownloader_WebAPI.Format.Csv);
            var csv = await sheetDownloader.DownloadAsync();

            // Parse
            progress = customProgress ?? 0.5f;
            EditorUtility.DisplayProgressBar(title, $"파싱중..({progress:P0})", progress);
            var rows = CsvReader.Read(csv);

            // Input
            progress = customProgress ?? 0.8f;
            EditorUtility.DisplayProgressBar(title, $"변환중..({progress:P0})", progress);
            scriptLines = new List<ScriptLine>();
            for (var i = 0; i < rows.Count; i++)
            {
                // 칼럼
                var columns = rows[i];

                // 빈 열 스킵
                if (columns.Count <= 1) continue;
                if (columns.All(string.IsNullOrEmpty)) continue;

                // 시나리오
                if (i == 2)
                {
                    ScnKey = columns[0];
                    ShowNotice = bool.Parse(columns[1]);
                    ScnName = columns[2];
                }

                // 대사
                if (i >= 5)
                {
                    scriptLines.Add(new ScriptLine
                    {
                        label = columns[0],
                        characterKey = columns[1],
                        text = columns[2],
                        voiceKey = columns[3]
                    });
                }
            }

            EditorUtility.SetDirty(this);
        }
        
        [ContextMenu(nameof(LoadGoogleSheet))] [Button]
        private async UniTaskVoid LoadGoogleSheet()
        {
            if (sheetDownloader == null)
            {
                Deb.LogWarning($"{nameof(sheetDownloader)} is null", this);
                return;
            }

            try
            {
                await LoadGoogleSheetTask();
                EditorGUIUtility.PingObject(this);
            }
            catch (Exception e)
            {
                Deb.LogException(e, this);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        
        // Maid Dialogue -> ScriptLine
        private async UniTask LoadScriptLines()
        {
            scriptLines = new List<ScriptLine>();

            for (int i = 0; i < dialogues.Count; ++i)
            {
                scriptLines.Add(new ScriptLine()
                {
                    label = "",
                    voiceKey = "",
                    characterKey = dialogues[i].player ? "player" : dialogues[i].who,
                    text = dialogues[i].script
                });
            }
            
            EditorUtility.SetDirty(this);
        }
        
        [ContextMenu(nameof(LoadDialogue))] [Button]
        private async UniTaskVoid LoadDialogue()
        {
            try
            {
                await LoadScriptLines();
                EditorGUIUtility.PingObject(this);
            }
            catch (Exception e)
            {
                Deb.LogException(e, this);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
#endif
}