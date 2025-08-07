using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Accelib.Editor.Utility.Discord;
using NaughtyAttributes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Accelib.Editor.AppInToss
{
    [CreateAssetMenu(fileName = "AutoBuildConfig_AIT", menuName = "Accelib/AutoBuildConfig_AIT")]
    public class AutoBuildConfig_AIT : ScriptableObject
    {
        [Header("# 디스코드 메세지")]
        [SerializeField] private bool sendDiscordMessage = true;
        [SerializeField, TextArea, ShowIf(nameof(sendDiscordMessage))] private string discordWebhookUrl = "https://discord.com/api/webhooks/1359366072817422466/2aUjhMzBL6vtczywmaNQxnajuJxBTFJs7dUCkQkCs3dHRfGb8gyB15Nmi-DqNoOFcYFn";
        
        [Header("# 프로젝트 설정")]
        [SerializeField] private string companyName;
        [SerializeField] private string productName;
        [SerializeField, ReadOnly] private string appVersion;

        [Header("# 빌드 설정")]
        [SerializeField] private bool isDev;
        [SerializeField, Range(0, 32)] private int buildIndex = 1;
        [SerializeField, ReadOnly] private string buildVersion;
        [SerializeField, ReadOnly] private string buildPath;
        
        [Header("# AIT 설정")]
        [SerializeField, ReadOnly] private string aitProjectFolder;
        [SerializeField, ReadOnly] private string aitBuildFolder;
        [SerializeField, ReadOnly] private string aitBuildName;
        [SerializeField] private string[] copyFolderNames = {"Build", "StreamingAssets", "TemplateData"};

        [Header("# WebGLTemplate")]
        [SerializeField, ReadOnly] private string accelibWebglTemplatePath = @"C:\WorkSpace\github.com\AccelixGames\Accelib\Packages\Accelib\WebGLTemplates\AccelixWeb";
        [SerializeField, ReadOnly] private string webglTemplatePath = Path.Combine(Application.dataPath, "WebGLTemplates", "AccelixWeb");
        

        private const string TemplateName = "AccelixWeb";
        
        private void OnEnable()
        {
            if(string.IsNullOrEmpty(companyName))
                companyName = PlayerSettings.companyName;
            if(string.IsNullOrEmpty(productName))
                productName = PlayerSettings.productName;
            
            UpdateVariables();
        }

        private void OnValidate()
        {
            UpdateVariables();
        }
        
        private void UpdateVariables()
        {
            var date = DateTime.Now.ToString("yyMMdd");
            appVersion = $"{date}-{buildIndex:D2}";

            var env = isDev ? "d" : "p";
            buildVersion = $"{env}{appVersion}";

            var projectFolder = Path.GetDirectoryName(Application.dataPath);
            buildPath = Path.Combine(projectFolder, "Builds", buildVersion);
            
            var aitProjectName = Path.GetFileName(aitProjectFolder);
            aitBuildName = string.IsNullOrEmpty(aitProjectName) ? "" : $"{aitProjectName}_{buildVersion}";
        }
        
        [Button("\U0001F4C1 [Select] Ait Project Folder")]
        private void SelectAitProjectFolder()
        {
            // 앱인토스의 프로젝트 폴더
            var path = EditorUtility.OpenFolderPanel("앱인토스 프로젝트 폴더 선택", aitProjectFolder, "");

            if (!string.IsNullOrEmpty(path))
            {
                aitProjectFolder = path;
                
                var aitProjectName = Path.GetFileName(aitProjectFolder);
                aitBuildName = $"{aitProjectName}_{buildVersion}";
            }
        }
        
        [Button("\U0001F4C1 [Select] Ait Build Folder")]
        private void SelectAitBuildFolder()
        {
            // 앱인토스의 빌드 파일(.ait)을 저장하는 폴더
            var path = EditorUtility.OpenFolderPanel("앱인토스 빌드 파일 저장 폴더 선택", aitBuildFolder, "");
            
            if (!string.IsNullOrEmpty(path))
            {
                aitBuildFolder = path;
            }
        }
        
        [Button("\U0001F5C2 [Open] AIT Project Folder")]
        private void OpenAITProjectFolder()
        {
            OpenFolderInExplorer(aitProjectFolder);
        }
        
        [Button("\U0001F5C2 [Open] AIT Build Folder")]
        private void OpenAITBuildFolder()
        {
            OpenFolderInExplorer(aitBuildFolder);
        }
        
        private static void OpenFolderInExplorer(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                Debug.LogError($"폴더가 존재하지 않습니다: {folderPath}");
                return;
            }

#if UNITY_EDITOR_WIN
            Process.Start("explorer.exe", folderPath.Replace("/", "\\"));
#elif UNITY_EDITOR_OSX
            Process.Start("open", folderPath);
#elif UNITY_EDITOR_LINUX
            Process.Start("xdg-open", folderPath);
#else
            UnityEngine.Debug.LogWarning("지원되지 않는 플랫폼입니다.");
#endif
        }


        [Button("\U0001F528 Build")]
        private void StartBuildProgress()
        {
            // 변수 업데이트
            UpdateVariables();
            // webgl template 복사
            CopyWebglTemplate();
            
            try
            {
                if (!Build_Webgl())
                    throw new Exception("WebGL 빌드 실패");
                if (!Build_AIT())
                    throw new Exception("AIT 빌드 실패");

                // ait 파일 있는 경로 열기
                OpenAITBuildFolder();
                
                // 인덱스 올리기
                buildIndex += 1;
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private bool Build_Webgl()
        {
            try
            {
                PlayerSettings.companyName = companyName;
                PlayerSettings.productName = productName;
                PlayerSettings.bundleVersion = appVersion;
                
                if (!ValidFolderPath(aitProjectFolder))
                    throw new Exception("ait 프로젝트 폴더의 경로가 유효하지 않습니다.");
                if (!ValidFolderPath(aitBuildFolder))
                    throw new Exception("ait 빌드 폴더의 경로가 유효하지 않습니다.");

                // 활성화 되어있는 씬
                var activeScenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
                if (activeScenes.Length <= 0)
                    throw new Exception("BuildSetting에 활성화된 Scene이 없습니다.");
                
                // 빌드 시작 메세지 발송
                var message = $":computer: ** 앱인토스 WebGL 빌드를 시작합니다!** [{AutoBuildConfig.GetNowTime()}]\n" +
                              $"{productName}({appVersion})";
                SendDiscordMessage(message);
                
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
                    options = BuildOptions.CompressWithLz4
                };
                
                // 빌드 폴더 생성
                if (!Directory.Exists(buildPath))
                    Directory.CreateDirectory(buildPath);

                // 빌드 시작
                var report = BuildPipeline.BuildPlayer(option);
                // 빌드 실패
                if (report == null)
                    throw new Exception($"WebGL 빌드 실패 : 빌드 리포트가 null 입니다.");

                var summary = report.summary;
                var embed = new JDiscordEmbed();
                
                // 빌드 실패
                if (summary.totalErrors > 0)
                {
                    var errors = new List<string>();
                
                    foreach (var step in report.steps)
                    {
                        var err = $"[{step.name}({step.depth} : {Mathf.RoundToInt((float)step.duration.TotalSeconds)}sec)]";

                        foreach (var stepMsg in step.messages)
                            if(stepMsg.type is LogType.Assert or LogType.Error or LogType.Exception)
                                errors.Add($"{err}{stepMsg.type} : {stepMsg.content}");
                    }

                    embed.title = $":warning: 빌드 실패 : ({productName})";
                    embed.description = $"{summary.totalErrors}개의 에러가 발생했습니다.\n";
                    foreach (var error in errors) 
                        embed.description += $"{error}\n";
                    SendDiscordMessage(null, embed);
                    
                    throw new Exception(embed.title);
                }
                
                // 빌드 성공
                var totalTime = $"{summary.totalTime.Minutes}분 {summary.totalTime.Seconds}초";
                embed.title = $"\u2705 빌드 성공: ({productName})\n";
                embed.description = $"- **버전**: {buildVersion}\n" +
                                    $"- **타겟**: {BuildTarget.WebGL}\n" +
                                    $"- **경로**: {summary.outputPath}\n" +
                                    $"- **소요시간**: {totalTime}\n";
                
                SendDiscordMessage(null, embed);

                Debug.Log($"{embed.title}\n{embed.description}");
            }
            catch (Exception e)
            {
                var embed = new JDiscordEmbed
                {
                    title = $":warning: 빌드 실패 : ({productName})",
                    description = $"- **실패 이유** : {e}"
                };
                SendDiscordMessage(null, embed);
                
                return false;
            }
            finally
            {
                SetLogTypes(StackTraceLogType.ScriptOnly);
            }

            return true;
        }

        private bool Build_AIT()
        {
            // 명령 실행
            var message = $":large_blue_diamond: **AIT 빌드를 시작합니다!** [{AutoBuildConfig.GetNowTime()}]";
            SendDiscordMessage(message);
            
            if (!CopyBuild()) return false;
            if (!Command()) return false;

            return true;
        }
        
        private bool CopyBuild()
        {
            try
            {
                var aitBuildPath = Path.Combine(aitProjectFolder, "build");
            
                // 폴더 내에 있는 파일 복사
                foreach (var folderName in copyFolderNames)
                {
                    var sourcePath = Path.Combine(buildPath, folderName);
                    var targetPath = Path.Combine(aitBuildPath, "public", folderName);
                
                    var result = CopyFiles(sourcePath, targetPath);
                    if (result < 0)
                        throw new Exception($"[{folderName}] 폴더 안에 있는 파일 복사에 실패했습니다.");
                }
                // src 복사
                var srcPath = Path.Combine(accelibWebglTemplatePath, "src");
                var aitSrcPath = Path.Combine(aitBuildPath, "src");
                var srcResult = CopyFiles(srcPath, aitSrcPath, ".meta");
                if (srcResult < 0)
                    throw new Exception($"[src] 폴더 안에 있는 파일 복사에 실패했습니다.");
                
                // index.html 복사
                var srcIndexFilePath = Path.Combine(buildPath, "index.html");
                var targetIndexFilePath = Path.Combine(aitBuildPath, "index.html");
                File.Copy(srcIndexFilePath, targetIndexFilePath, overwrite: true);
            }
            catch (Exception e)
            {
                var embed = new JDiscordEmbed
                {
                    title = $":warning: 빌드 실패 : ({aitBuildName})",
                    description = $"- **실패 이유** : {e}"
                };
                SendDiscordMessage(null, embed);
                
                Debug.LogException(e);
                return false;
            }
            
            return true;
        }
        
        private static int CopyFiles(string sourceDir, string targetDir, params string[] excludeExtensions)
        {
            try
            {
                if (!Directory.Exists(sourceDir))
                {
                    Debug.LogWarning($"원본 폴더가 존재하지 않습니다: {sourceDir}");
                    return 1;
                }

                // 대상 폴더 생성 또는 초기화
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
                else
                {
                    var oldFiles = Directory.GetFiles(targetDir);
                    foreach (var oldFile in oldFiles)
                    {
                        File.Delete(oldFile);
                    }
                }

                var newFiles = Directory.GetFiles(sourceDir);
                int copiedCount = 0;

                foreach (var srcFilePath in newFiles)
                {
                    string ext = Path.GetExtension(srcFilePath);
                    if (excludeExtensions != null && excludeExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                        continue;

                    var fileName = Path.GetFileName(srcFilePath);
                    var destFilePath = Path.Combine(targetDir, fileName);
                    File.Copy(srcFilePath, destFilePath, overwrite: true);
                    copiedCount++;
                }

                Debug.Log($"📁 복사 완료: {copiedCount}개 파일 (.제외 확장자: {string.Join(", ", excludeExtensions ?? Array.Empty<string>())})\n" +
                          $"- 원본: {sourceDir}\n- 대상: {targetDir}");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return -1;
            }

            return 2;
        }

        private bool Command()
        {
            try
            {
                RunCommand("yarn upgrade --latest", aitProjectFolder);
                RunCommand("yarn build", aitProjectFolder);
            
                var aitFileName = $"{Path.GetFileName(aitProjectFolder)}.ait";
                var altFilePath = Path.Combine(aitProjectFolder, aitFileName);
            
                // .ait 빌드 실패
                if (!File.Exists(altFilePath))
                    throw new Exception(".ait 빌드 파일 생성에 실패했습니다.");
            
                // .ait 빌드 성공 > 파일 이동
                var targetFilePath = Path.Combine(aitBuildFolder, $"{aitBuildName}.ait");
            
                File.Copy(altFilePath, targetFilePath, overwrite: true);
                File.Delete(altFilePath);
                
                var embed = new JDiscordEmbed
                {
                    title = $"\u2705 빌드 성공: ({aitBuildName})\n",
                    description = $"- **버전**: {buildVersion}\n" +
                                  $"- **타겟**: AIT\n" +
                                  $"- **경로** : {targetFilePath}\n",
                };
                SendDiscordMessage(null, embed);
            }
            catch (Exception e)
            {
                var embed = new JDiscordEmbed
                {
                    title = $":warning: 빌드 실패 : ({aitBuildName})",
                    description = $"- **실패 이유** : {e}"
                };
                SendDiscordMessage(null, embed);

                Debug.LogException(e);
                return false;
            }

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

        private void CopyWebglTemplate()
        {
            CopyFiles(accelibWebglTemplatePath, webglTemplatePath);

            var srcTemplateData = Path.Combine(accelibWebglTemplatePath, "TemplateData");
            var targetTemplateData = Path.Combine(webglTemplatePath, "TemplateData");
            CopyFiles(srcTemplateData, targetTemplateData);

            Debug.Log("Webgl Template 복사!");
        }
        
        private void SendDiscordMessage(string content, params JDiscordEmbed[] embeds)
        {
            if(sendDiscordMessage)
                DiscordWebhook.SendMsg(discordWebhookUrl, content, embeds);
        }
    }
}
