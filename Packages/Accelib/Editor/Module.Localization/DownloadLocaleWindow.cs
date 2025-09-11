using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.EditorTool.Google;
using Accelib.EditorTool.Google.Control.Sheets;
using Accelib.EditorTool.Google.Control.Utility;
using Accelib.Extensions;
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
        private const int FirstRow = 2;
        
        private static readonly Vector2 MinSize = new(400, 500);
        private static readonly Vector2 MaxSize = new(800, 500);

        private EditorObjectField<GoogleSheetsDownloaderBase> _sheetDownloader;
        private EditorObjectField<LocaleSO> _koreanLocale;
        private EditorObjectField<LocaleSO> _englishLocale;
        private EditorObjectField<LocaleSO> _japaneseLocale;
        private EditorObjectField<LocaleSO> _chineseTraditionalLocale;
        private EditorObjectField<LocaleSO> _chineseSimplifiedLocale;

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
            _sheetDownloader = new EditorObjectField<GoogleSheetsDownloaderBase>("Locale Sheet");
            _koreanLocale = new EditorObjectField<LocaleSO>(SystemLanguage.Korean.ToString());
            _englishLocale = new EditorObjectField<LocaleSO>(SystemLanguage.English.ToString());
            _japaneseLocale = new EditorObjectField<LocaleSO>(SystemLanguage.Japanese.ToString());
            _chineseSimplifiedLocale = new EditorObjectField<LocaleSO>(SystemLanguage.ChineseSimplified.ToString());
            _chineseTraditionalLocale = new EditorObjectField<LocaleSO>(SystemLanguage.ChineseTraditional.ToString());

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
                _japaneseLocale.GUILayout("일본어");
                _chineseSimplifiedLocale.GUILayout("중문(간체)");
                _chineseTraditionalLocale.GUILayout("중문(번체)");
                
                if (GUILayout.Button("다운로드")) 
                    OnClickDownload().Forget();
            
                if (check.changed)
                {
                    _sheetDownloader.SaveOnChanged();
                    _koreanLocale.SaveOnChanged();
                    _englishLocale.SaveOnChanged();
                    _japaneseLocale.SaveOnChanged();
                    _chineseSimplifiedLocale.SaveOnChanged();
                    _chineseTraditionalLocale.SaveOnChanged();
                
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
            if (_koreanLocale == null && _englishLocale == null && _japaneseLocale == null &&
                _chineseSimplifiedLocale == null && _chineseTraditionalLocale == null)
                return BeepWarning("연결된 언어 데이터가 없습니다.");

            const string dialogueTitle = "로케일 다운로더";
            EditorUtility.DisplayProgressBar(dialogueTitle, $"csv 다운로드 중..", 0f);

            try
            {
                var sheet = await _sheetDownloader.asset.DownloadAsSheetDataAsync();
                if(sheet == null)
                    throw new Exception("다운로드한 데이터가 비었습니다.");
                
                EditorUtility.DisplayProgressBar(dialogueTitle, "스크립트 다이어로그 파싱중..", 0.5f);

                var koDict = new Dictionary<string, string>();
                var enDict = new Dictionary<string, string>();
                var jaDict = new Dictionary<string, string>();
                var zhchDict = new Dictionary<string, string>();
                var zhtwDict = new Dictionary<string, string>();

                var parsedArray = sheet.values;
                var rowCount = parsedArray.Count;
                
                for (var i = FirstRow; i < rowCount; i++)
                {
                    // 행 한줄 구하기
                    var parsed = parsedArray[i];
                
                    // 빈 열 스킵
                    if (parsed.All(string.IsNullOrEmpty)) continue;
                
                    // 키
                    var key = parsed.GetOrDefault(0);
                    // 노트
                    // var note = parsed.GetOrDefault(1);
                    // 각 언어별로 딕셔너리 작성
                    koDict[key] =   parsed.GetOrDefault(2);
                    enDict[key] =   parsed.GetOrDefault(3);
                    jaDict[key] =   parsed.GetOrDefault(4);
                    zhchDict[key] = parsed.GetOrDefault(5);
                    zhtwDict[key] = parsed.GetOrDefault(6);
                }
                
                _koreanLocale?.asset?.FromDictionary(koDict);
                _englishLocale?.asset?.FromDictionary(enDict);
                _japaneseLocale?.asset?.FromDictionary(jaDict);
                _chineseSimplifiedLocale?.asset?.FromDictionary(zhchDict);
                _chineseTraditionalLocale?.asset?.FromDictionary(zhtwDict);
            }
            catch (Exception e)
            {
                return BeepWarning(e.Message);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            
            EditorGUIUtility.PingObject(_koreanLocale?.asset);
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