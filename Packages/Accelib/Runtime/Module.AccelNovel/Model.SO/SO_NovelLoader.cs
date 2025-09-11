#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accelib.EditorTool.Google.Control.Sheets;
using Accelib.EditorTool.Google.Control.Utility;
using Accelib.Logging;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace Accelib.Module.AccelNovel.Model.SO
{
    [CreateAssetMenu(fileName = "novelLoader", menuName = "Accelib/AccelNovel/NovelLoader", order = 1)]
    public class SO_NovelLoader : ScriptableObject
    {
        [SerializeField] private SO_NovelConfig config;
        
        [Header("캐릭터")]
        [SerializeField] private GoogleSheetsDownloader_WebAPI charDownloader;
        [SerializeField, ReadOnly, TextArea] private string savePath;

        [Button]
        private void SetCharacterSavePath()
        {
            var path = EditorUtility.OpenFolderPanel("폴더 선택", Application.dataPath, "");
            if(!string.IsNullOrEmpty(path))
                savePath = "Assets" + path.Replace(Application.dataPath, "");
                
            EditorUtility.SetDirty(this);
        }
        
        [Button]
        private async UniTaskVoid LoadCharacter()
        {
            if (!AssetDatabase.IsValidFolder(savePath))
            {
                Debug.LogWarning($"{savePath} 는 올바른 에셋 경로가 아닙니다.");
                return;
            }
            
            if (charDownloader == null)
            {
                Debug.LogWarning($"{nameof(charDownloader)} is null");
                return;
            }
            
            try
            {
                const string title = "로딩중.."; 
                
                // Download
                EditorUtility.DisplayProgressBar(title, "데이터 다운로드중..", 0f);
                charDownloader.SetFormat(GoogleSheetsDownloader_WebAPI.Format.Csv);
                var csv = await charDownloader.DownloadAsync();

                // Parse
                EditorUtility.DisplayProgressBar(title, "파싱중..", 0.5f);
                var rows = CsvReader.Read(csv);

                // 리스트 생성
                var charList = new List<SO_Character>();
                foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(savePath))
                    if (asset is SO_Character character)
                    {
                        charList.Add(character);
                        character.resourceKeyDict ??= new AYellowpaper.SerializedCollections.SerializedDictionary<string, string>();
                        character.resourceKeyDict.Clear();
                    }
                
                // 로드
                EditorUtility.DisplayProgressBar(title, "변환중..", 0.8f);
                for (var i = 2; i < rows.Count; i++)
                {
                    // 칼럼
                    var columns = rows[i];

                    // 빈 열 스킵
                    if (columns.Count <= 1) continue;
                    if (columns.All(string.IsNullOrEmpty)) continue;

                    var baseKey = columns[0];
                    var displayName = columns[1];
                    var emotion = columns[2];
                    var resource = columns[3];

                    var charSO = charList.FirstOrDefault(x => x.key == baseKey);
                    if (charSO == null)
                    {
                        charSO = CreateInstance<SO_Character>();
                        charSO.key = baseKey;
                        charList.Add(charSO);
                        
                        var assetName = $"char_{baseKey}.asset";
                        AssetDatabase.CreateAsset(charSO, Path.Combine(savePath, assetName));
                        Deb.Log($"Asset Created: {assetName}");
                    }
                    
                    charSO.displayName = displayName;
                    
                    if (!string.IsNullOrEmpty(resource))
                    {
                        var folder = char.ToUpper(baseKey[0]) + baseKey[1..];
                        var dicedResource = $"{folder}/diced_char_{baseKey}/{resource}.asset";
                        charSO.resourceKeyDict.Add(emotion, dicedResource);
                        Deb.Log($"Asset Updated({charSO.name}): {emotion}:{dicedResource}");
                    }
                    
                    EditorUtility.SetDirty(charSO);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
#endif