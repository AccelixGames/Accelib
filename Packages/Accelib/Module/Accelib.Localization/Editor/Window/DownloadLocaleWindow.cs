#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.EditorTool.Google.Control.Sheets;
using Accelib.Module.Localization.Architecture;
using Accelib.Editor.Module.Localization.Utility;
using Accelib.Extensions;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Accelib.Editor.Module.Localization
{
    /// <summary>
    /// Google Sheets에서 로케일 데이터를 다운로드하는 에디터 윈도우.
    /// </summary>
    public class DownloadLocaleWindow : EditorWindow
    {
        private const int FirstRow = 4;

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
            _koreanLocale = new EditorObjectField<LocaleSO>(nameof(SystemLanguage.Korean));
            _englishLocale = new EditorObjectField<LocaleSO>(nameof(SystemLanguage.English));
            _japaneseLocale = new EditorObjectField<LocaleSO>(nameof(SystemLanguage.Japanese));
            _chineseSimplifiedLocale = new EditorObjectField<LocaleSO>(nameof(SystemLanguage.ChineseSimplified));
            _chineseTraditionalLocale = new EditorObjectField<LocaleSO>(nameof(SystemLanguage.ChineseTraditional));

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

                foreach (var range in sheet.valueRanges)
                {
                    var parsedArray = range.values;
                    for (var i = FirstRow; i < parsedArray.Count; i++)
                    {
                        // 행 한줄 구하기
                        var parsed = parsedArray[i];

                        // 빈 열 스킵
                        if (parsed.All(string.IsNullOrEmpty)) continue;

                        // 키
                        var keyId = 2;
                        var key = parsed.GetOrDefault(keyId);
                        if (string.IsNullOrEmpty(key)) continue;

                        // 각 언어별로 딕셔너리 작성
                        koDict[key] = parsed.GetOrDefault(keyId + 1);
                        enDict[key] = parsed.GetOrDefault(keyId + 2);
                        jaDict[key] = parsed.GetOrDefault(keyId + 3);
                        zhchDict[key] = parsed.GetOrDefault(keyId + 4);
                        zhtwDict[key] = parsed.GetOrDefault(keyId + 5);
                    }
                }

                _koreanLocale?.asset?.FromDictionary(koDict);
                _englishLocale?.asset?.FromDictionary(enDict);
                _japaneseLocale?.asset?.FromDictionary(jaDict);
                _chineseSimplifiedLocale?.asset?.FromDictionary(zhchDict);
                _chineseTraditionalLocale?.asset?.FromDictionary(zhtwDict);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
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
#endif
