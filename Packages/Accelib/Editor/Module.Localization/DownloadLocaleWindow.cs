using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.EditorTool.GoogleSheet;
using Accelib.Logging;
using Accelib.Module.Localization.Architecture;
using Accelix.Editor;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Accelib.Editor.Module.Localization
{
    public class DownloadLocaleWindow : EditorWindow
    {
        private static readonly Vector2 MinSize = new(400, 500);
        private static readonly Vector2 MaxSize = new(800, 500);

        private EditorObjectField<GoogleSheetDownloader> _sheetDownloader;
        private EditorObjectField<LocaleSO> _koreanLocale;
        private EditorObjectField<LocaleSO> _englishLocale;
        // private EditorObjectField<LocaleSO> _japaneseLocale;
        // private EditorObjectField<LocaleSO> _chineseTraditionalLocale;
        // private EditorObjectField<LocaleSO> _chineseSimplifiedLocale;

        [MenuItem("Accelix/Download Locale")]
        private static void Open()
        {
            var window = GetWindow<DownloadLocaleWindow>();
            window.titleContent = new GUIContent("로케일 다운로드");
            window.minSize = MinSize;
            window.maxSize = MaxSize;

            window.Show();
        }

        private void Initialize()
        {
            _sheetDownloader = new EditorObjectField<GoogleSheetDownloader>("Locale Sheet");
            _koreanLocale = new EditorObjectField<LocaleSO>(SystemLanguage.Korean.ToString());
            _englishLocale = new EditorObjectField<LocaleSO>(SystemLanguage.English.ToString());
            // _japaneseLocale = new EditorObjectField<LocaleSO>(SystemLanguage.Japanese.ToString());
            // _chineseSimplifiedLocale = new EditorObjectField<LocaleSO>(SystemLanguage.ChineseSimplified.ToString());
            // _chineseTraditionalLocale = new EditorObjectField<LocaleSO>(SystemLanguage.ChineseTraditional.ToString());

            Repaint();
        }

        private void OnValidate() => Initialize();
        private void OnEnable() => Initialize();

        private void OnGUI()
        {
            using (new GUILayout.VerticalScope())
            {
                using var check = new EditorGUI.ChangeCheckScope();
                
                _sheetDownloader.GUILayout("구글 시트 데이터");
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("언어 데이터 연결", EditorStyles.boldLabel);
                _koreanLocale.GUILayout("한글");
                _englishLocale.GUILayout("영어");
                // _japaneseLocale.GUILayout("일본어");
                // _chineseSimplifiedLocale.GUILayout("중문(간체)");
                // _chineseTraditionalLocale.GUILayout("중문(번체)");

                
                if (GUILayout.Button("다운로드")) 
                    OnClickDownload().Forget();
            
                if (check.changed)
                {
                    _sheetDownloader.SaveOnChanged();
                    _koreanLocale.SaveOnChanged();
                    _englishLocale.SaveOnChanged();
                    // _japaneseLocale.SaveOnChanged();
                    // _chineseSimplifiedLocale.SaveOnChanged();
                    // _chineseTraditionalLocale.SaveOnChanged();
                
                    Repaint();
                }
            }
            
            if (Event.current.type == EventType.MouseDown)
            {
                EditorGUI.FocusTextInControl("");
                Repaint();
            }
        }
        
        private async UniTask<bool> OnClickDownload()
        {
            if(_sheetDownloader == null) return BeepWarning("구글 시트 다운로더가 없습니다.");
            if(_koreanLocale == null) return BeepWarning("한글이 연결되지 않았습니다.");
            if(_englishLocale == null) return BeepWarning("영어가 연결되지 않았습니다.");
            // if(_japaneseLocale == null) return BeepWarning("일본어가 연결되지 않았습니다.");
            // if(_chineseSimplifiedLocale == null) return BeepWarning("중국어(간체)가 연결되지 않았습니다.");
            // if(_chineseTraditionalLocale == null) return BeepWarning("중국어(번체)가 연결되지 않았습니다.");

            const string dialogueTitle = "로케일 다운로더";
            EditorUtility.DisplayProgressBar(dialogueTitle, $"tsv 다운로드 중..", 0f);

            try
            {
                var csv = await _sheetDownloader.asset.DownloadAsync();
                if(string.IsNullOrEmpty(csv))
                    throw new Exception("다운로드한 CSV 가 비었습니다.");
                
                EditorUtility.DisplayProgressBar(dialogueTitle, "스크립트 다이어로그 파싱중..", 0.5f);

                var koDict = new Dictionary<string, string>();
                var enDict = new Dictionary<string, string>();
                // var jaDict = new Dictionary<string, string>();
                // var zhchDict = new Dictionary<string, string>();
                // var zhtwDict = new Dictionary<string, string>();

                var parsedArray = CsvReader.Read(csv);
                var rowCount = parsedArray.Count;
                
                for (var i = 3; i < rowCount; i++)
                {
                    // 행 한줄 구하기
                    var parsed = parsedArray[i];
                    //
                    // // 13번 아스키 문자 삭제 (이상한 줄바꿈)
                    // var nullId = rowList.LastIndexOf((char)13);
                    // if(nullId > 0)
                    //     rowList = rowList.Remove(nullId, 1);
                    //
                    // // 행을 한칸씩 파싱
                    // var parsed = rowList.Split('\t');
                
                    // 빈 열 스킵
                    if (parsed.Count != 4) continue;
                    if (parsed.All(string.IsNullOrEmpty)) continue;
                
                    // 키
                    var key = parsed[0];
                    // 노트
                    var note = parsed[1];
                    // 각 언어별로 딕셔너리 작성
                    koDict[key] = parsed[2];
                    enDict[key] = parsed[3];
                    // Deb.Log($"{key}: {parsed[2]}, {parsed[3]}");
                    // jaDict[key] = parsed[4];
                    // zhchDict[key] = parsed[5];
                    // zhtwDict[key] = parsed[6];
                }
                
                _koreanLocale.asset.FromDictionary(koDict);
                _englishLocale.asset.FromDictionary(enDict);
                // _japaneseLocale.asset.FromDictionary(jaDict);
                // _chineseSimplifiedLocale.asset.FromDictionary(zhchDict);
                // _chineseTraditionalLocale.asset.FromDictionary(zhtwDict);
            }
            catch (Exception e)
            {
                return BeepWarning(e.Message);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            
            EditorGUIUtility.PingObject(_koreanLocale.asset);
            return true;
        }

        private bool BeepWarning(string msg)
        {
            EditorApplication.Beep();
            EditorUtility.DisplayDialog("오류", msg, "확인");
            
            return false;
        }
    }
}