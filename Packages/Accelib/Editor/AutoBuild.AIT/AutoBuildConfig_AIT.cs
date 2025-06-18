using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NaughtyAttributes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace Accelib.Editor
{
    [CreateAssetMenu(fileName = "AutoBuildConfig_AIT", menuName = "Accelib/AutoBuildConfig_AIT")]
    public class AutoBuildConfig_AIT : ScriptableObject
    {
        [Header("# 프로젝트 설정")]
        [SerializeField] private string companyName;
        [SerializeField] private string productName;
        [SerializeField, ReadOnly] private string appVersion;

        [Header("# 빌드 설정")]
        [SerializeField] private bool isDev;
        [SerializeField] private int index = 1;
        [SerializeField, ReadOnly] private string buildVersion;
        [SerializeField, ReadOnly] private string buildPath;
        
        [Header("# AIT 설정")]
        [SerializeField, ReadOnly] private string aitProjectFolder;
        [SerializeField, ReadOnly] private string aitBuildFolder;
        [SerializeField, ReadOnly] private string aitBuildName;

        [SerializeField] private string[] copyFolderNames = {"Build", "StreamingAssets", "TemplateData"};
        
        
        [Button("\U0001F4C1 Ait Project Folder")]
        private void SelectAitProjectFolder()
        {
            // 앱인토스의 프로젝트 폴더
            var path = EditorUtility.OpenFolderPanel("앱인토스 프로젝트 폴더 선택", "", "");

            if (!string.IsNullOrEmpty(path))
            {
                aitProjectFolder = path;
                
                var aitProjectName = Path.GetFileName(aitProjectFolder);
                aitBuildName = $"{aitProjectName}-{buildVersion}";
            }
        }
        
        [Button("\U0001F4C1 Ait Build Folder")]
        private void SelectAitBuildFolder()
        {
            // 앱인토스의 빌드 파일(.ait)을 저장하는 폴더
            var path = EditorUtility.OpenFolderPanel("앱인토스 빌드 파일 저장 폴더 선택", "", "");
            
            if (!string.IsNullOrEmpty(path))
            {
                aitBuildFolder = path;
            }
        }

        [Button("\U0001F528 Build")]
        private void StartBuild()
        {
            PlayerSettings.companyName = companyName;
            PlayerSettings.productName = productName;
            PlayerSettings.bundleVersion = appVersion;

            UpdateVariables();

            try
            {
                if (!Build())
                    throw new Exception("WebGL Build 실패");
                if (!Copy())
                    throw new Exception("WebGL -> AIT 프로젝트로 파일 복사 실패");
                if(!Command())
                    throw new Exception("AIT 빌드 실패");
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private bool Build()
        {
            try
            {
                if (!ValidFolderPath(aitProjectFolder))
                    throw new Exception("ait 프로젝트 폴더의 경로가 유효하지 않습니다.");
                if (!ValidFolderPath(aitBuildFolder))
                    throw new Exception("ait 빌드 폴더의 경로가 유효하지 않습니다.");

                // 활성화 되어있는 씬
                var activeScenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
                if (activeScenes.Length <= 0)
                    throw new Exception("BuildSetting에 활성화된 Scene이 없습니다.");
                
                // 현재 씬 모두 저장
                EditorSceneManager.SaveOpenScenes();

                // 버전 설정
                PlayerSettings.bundleVersion = appVersion;

                // 스택트레이스 초기화
                SetLogTypes(StackTraceLogType.None);
                
                // 빌드 옵션
                var option = new BuildPlayerOptions
                {
                    scenes = activeScenes,
                    locationPathName = buildPath,
                    target = BuildTarget.WebGL,
                    options = BuildOptions.CompressWithLz4HC
                };
                
                // 빌드 폴더 생성
                if (!Directory.Exists(buildPath))
                    Directory.CreateDirectory(buildPath);

                // 빌드 시작
                var report = BuildPipeline.BuildPlayer(option);
                // 빌드 실패
                if (report == null)
                    throw new Exception($"WebGL 빌드 실패 : 빌드 리포트가 null 입니다.");

                // 빌드 성공
                var summary = report.summary;
                var totalTime = $"{summary.totalTime.Minutes}분 {summary.totalTime.Seconds}초";
                var msg = $"\u2705 빌드 성공: {productName}-{buildVersion}\n" +
                          $"- 경로: {summary.outputPath}\n" +
                          $"- 소요시간: {totalTime}\n";

                Debug.Log(msg);

                SetLogTypes(StackTraceLogType.ScriptOnly);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);

                return false;
            }
            finally
            {
                SetLogTypes(StackTraceLogType.ScriptOnly);
            }

            return true;
        }
        
        private bool Copy()
        {
            var aitPath = Path.Combine(aitProjectFolder, "build");
            
            // 폴더 내에 있는 파일 복사
            foreach (var folderName in copyFolderNames)
            {
                var sourcePath = Path.Combine(buildPath, folderName);
                var targetPath = Path.Combine(aitPath, "public", folderName);
                
                var result = CopyFiles(sourcePath, targetPath);
                if (!result)
                    return false;
            }
            // index.html 복사
            var srcIndexFilePath = Path.Combine(buildPath, "index.html");
            var targetIndexFilePath = Path.Combine(aitPath, "index.html");
            File.Copy(srcIndexFilePath, targetIndexFilePath, overwrite: true);

            var msg = $"\u2705 [webgl] > [ait] 프로젝트로 파일 복사 성공: {productName}-{buildVersion}\n" +
                      $"- webgl 빌드 경로: {buildPath}\n" +
                      $"- ait 빌드 경로 : {aitPath}\n";
            
            Debug.Log(msg);
            
            return true;
        }
        
        private static bool CopyFiles(string sourceDir, string targetDir)
        {
            try
            {
                // 빌드 경로에 있는 파일 복사 > 앱인토스 프로젝트 의 폴더들에 이동
                if (!Directory.Exists(sourceDir))
                {
                    throw new Exception($"원본 폴더가 존재하지 않습니다: {sourceDir}");
                }

                // 대상 폴더가 없을 경우 폴더 생성
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
                else
                {
                    // 대상 폴더의 모든 파일 삭제
                    var oldFiles = Directory.GetFiles(targetDir);
                    foreach (var oldFile in oldFiles)
                    {
                        File.Delete(oldFile);
                    }
                }

                // 원본 폴더의 모든 파일 복사
                var newFiles = Directory.GetFiles(sourceDir);
                foreach (var srcFilePath in newFiles)
                {
                    var fileName = Path.GetFileName(srcFilePath);
                    var destFilePath = Path.Combine(targetDir, fileName);
                    File.Copy(srcFilePath, destFilePath, overwrite: true);
                }

                var msg = $"파일 복사 완료 : {newFiles.Length}개의 파일\n" +
                          $"- 원본 경로 : {sourceDir}\n"+
                          $"- 대상 경로 : {targetDir}\n";
            
                Debug.Log(msg);
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                return false;
            }

            return true;
        }
        
        private bool Command()
        {
            // 명령 실행
            Debug.Log("ait 빌드 시작...");
            
            RunCommand("yarn upgrade --latest", aitProjectFolder);
            RunCommand("yarn build", aitProjectFolder);
            
            var aitFileName = $"{Path.GetFileName(aitProjectFolder)}.ait";
            var altFilePath = Path.Combine(aitProjectFolder, aitFileName);
            
            // .ait 빌드 실패
            if (!File.Exists(altFilePath)) return false;
            
            // .ait 빌드 성공 > 파일 이동
            var targetFilePath = Path.Combine(aitBuildFolder, $"{aitBuildName}.ait");
            
            File.Copy(altFilePath, targetFilePath, overwrite: true);
            File.Delete(altFilePath);
            
            var msg = $"\u2705 ait 파일 빌드 성공: {productName}-{buildVersion}\n" +
                         $"- ait 파일 경로 : {targetFilePath}\n";
            Debug.Log(msg);

            return true;
        }
        
        private static void RunCommand(string command, string workingDirectory)
        {
            var startInfo = new ProcessStartInfo
            {
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
            };
            
            // 명령 셀 지정
#if UNITY_EDITOR_WIN
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments  = $"/c {command}";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
            startInfo.FileName = "/bin/bash";
            startInfo.Arguments  = $"-c \"{command}\"";
#endif   
            
            using (var process = new Process())
            {
                process.StartInfo = startInfo;
                
                // process 로그 
                //process.OutputDataReceived += (sender, e) => { if (e.Data != null) Debug.Log(e.Data); };
                //process.ErrorDataReceived += (sender, e) => { if (e.Data != null) Debug.LogError(e.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }
        
        private void SetLogTypes(StackTraceLogType stackTraceLogType)
        {
            for (var i = 0; i <= (int)LogType.Exception; i++) 
                PlayerSettings.SetStackTraceLogType((LogType)i, stackTraceLogType);
        }
        
        private bool ValidFolderPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && Directory.Exists(path);
        }

        private void UpdateVariables()
        {
            var date = DateTime.Now.ToString("yyMMdd");
            appVersion = $"{date}.{index:D2}";

            var env = isDev ? "d" : "p";
            buildVersion = $"{env}{appVersion}";

            var projectFolder = Path.GetDirectoryName(Application.dataPath);
            buildPath = Path.Combine(projectFolder, "Builds", buildVersion);
            
            var aitProjectName = Path.GetFileName(aitProjectFolder);
            aitBuildName = string.IsNullOrEmpty(aitProjectName) ? "" : $"{aitProjectName}-{buildVersion}";
        }
        
        private void OnEnable()
        {
            companyName = PlayerSettings.companyName;
            productName = PlayerSettings.productName;
            
            UpdateVariables();
        }

        private void OnValidate()
        {
            UpdateVariables();
        }
    }
}
