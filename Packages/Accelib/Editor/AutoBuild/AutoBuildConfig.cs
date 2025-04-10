using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accelib.Editor.Architecture;
using Accelib.Editor.AutoBuild.Steamwork;
using Accelib.Editor.Steamwork;
using Accelib.Editor.Utility.Discord;
using NaughtyAttributes;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Accelib.Editor
{
    [CreateAssetMenu(fileName = "AutoBuildConfig", menuName = "Accelib/AutoBuildConfig", order = 0)]
    public class AutoBuildConfig : ScriptableObject
    {
        private static string SDKPathKey => $"{nameof(AutoBuildConfig)}_SDKPath";
        
        [Header("메신저 옵션")]
        [SerializeField] private bool sendDiscordMessage = true;
        [SerializeField, TextArea] private string discordWebhookUrl = "https://discord.com/api/webhooks/1312969930349740043/LjGysdGxpCjvdJT7YPK8d95YF9Nq9zikqO2sytt2GW7khn6LTrAJoAGTa7QXQDv_Q5c4";

        [Header("계정 및 앱")]
        [SerializeField] private string username;
        [SerializeField] private List<AppConfig> apps;

        [Header("Stage")] 
        [SerializeField] private bool skipBuild = false;
        [SerializeField] private bool skipUpload = false;
        
        [Header("빌드 옵션")]
        [SerializeField, ReadOnly] private string sdkPath;
        [SerializeField, ReadOnly] private string appName;
        [SerializeField, ReadOnly] private string versionText;
        [SerializeField] private Vector3Int version = new (1, 0, 0);
        [SerializeField, TextArea] private string patchNote;
        
        [Button]
        public void LoadSDKPath()
        {
            var path = EditorUtility.OpenFolderPanel("Select steamwork folder", "", "");

            if (!string.IsNullOrEmpty(path))
            {
                sdkPath = path;
                    
                EditorPrefs.SetString(SDKPathKey, sdkPath);
                EditorUtility.SetDirty(this);
            }
        }
        
        [Button]
        public void StartBuildProcess()
        {
            // 초기 플레이어 캐싱
            var initialBuildProfile = BuildProfile.GetActiveBuildProfile();
            
            try
            {
                // SDK Path is Empty
                if (!skipUpload && string.IsNullOrEmpty(sdkPath))
                    throw new Exception("SDK Path is empty");

                // Discord URL is Empty
                if (sendDiscordMessage && string.IsNullOrEmpty(discordWebhookUrl))
                    throw new Exception("DiscordWebhookUrl is empty");
                
                // 현재 씬 모두 저장
                EditorSceneManager.SaveOpenScenes();
                
                // 버전 설정
                var versionStr = $"{version.x}.{version.y:D2}.{version.z:D2}";
                var versionNumber = (version.x * 10000 + version.y * 100 + version.z);
                PlayerSettings.bundleVersion = versionStr;
                versionText = versionStr;
                PlayerSettings.Android.bundleVersionCode = versionNumber;
                PlayerSettings.iOS.buildNumber = versionNumber.ToString();
                PlayerSettings.macOS.buildNumber = versionNumber.ToString();
                
                // 스택트레이스 초기화
                SetLogTypes(StackTraceLogType.None);
                
                // 기본 경로 구하기
                var basePath = Path.Combine(sdkPath, "tools", "ContentBuilder", "content");

                // 빌드 풀 생성
                var buildInfos = new List<BuildInfo>();
                var uploadInfos = new List<UploadInfo>();

                // 앱을 순회하며,
                appName = Application.productName;
                foreach (var app in apps)
                {
                    // 앱 경로 구하기
                    var appPath = Path.Combine(basePath, app.name);
                    var baseBuildPath = Path.Combine(appPath, "build");
                    var baseLogPath = Path.Combine(appPath, "log");
                    var scriptPath = Path.Combine(appPath, "scripts");

                    // 패치노트
                    var patchNotePreInfo = $"[{app.name}({versionStr})-";
                    
                    // 디포를 순회하며,
                    foreach (var depot in app.depots)
                    {
                        if (!depot.includeInBuild) continue;

                        // Platform
                        var platform = depot.buildTarget.ToString();
                        var buildPath = Path.Combine(baseBuildPath, platform, "v" + versionNumber);
                        var fileName = $"{appName}{GetAppExecutionExtension(depot.buildTarget)}";

                        // 디포 빌드 스크립트 생성
                        var depotVdfPath = Path.Combine(scriptPath, $"depot_{depot.depotID}.vdf");
                        var depotContent = DepotUtility.GetDepotContent(appName, depot.depotID, buildPath);
                        DepotUtility.CreateFile(depotVdfPath, depotContent);
                        
                        // 빌드 정보 추가
                        buildInfos.Add(new BuildInfo
                        {
                            app = app,
                            depot = depot,
                            versionStr = versionStr,
                            versionNumber = versionNumber.ToString(),
                            buildPath = Path.Combine(buildPath, fileName)
                        });
                        
                        // 패치노트
                        patchNotePreInfo += $"{platform}, ";
                    }
                    
                    // 앱 빌드 스크립트 생성
                    var appVdfPath = Path.Combine(scriptPath, $"app_{app.appID}.vdf");
                    var desc = $"{patchNotePreInfo}]  " + patchNote;
                    var logPath = Path.Combine(baseLogPath, "v" + versionNumber);
                    var appContent = DepotUtility.GetAppContent(app.appID, desc, logPath, app.liveBranch, app.depots);
                    DepotUtility.CreateFile(appVdfPath, appContent);
                    
                    // 업로드 정보 추가
                    uploadInfos.Add(new UploadInfo
                    {
                        app = app,
                        logPath = logPath,
                        vdfPath = appVdfPath
                    });
                }

                // 빌드 풀이 없으면 에러
                if (buildInfos.Count == 0)
                    throw new Exception("No build pool found");

                // 플랫폼으로 빌드 풀 정렬
                buildInfos.Sort((a, b) => a.depot.buildTarget.CompareTo(b.depot.buildTarget));

                if (!skipBuild)
                {
                    // 디스코드 메세지 생성
                    var msg = $":computer: **빌드를 시작합니다!** [{GetNowTime()}]\n";
                    foreach (var buildInfo in buildInfos)
                        msg += $"- {buildInfo.app.name} | {buildInfo.depot.buildTarget} | {buildInfo.versionStr}\n";
                    if(sendDiscordMessage)
                        DiscordWebhook.SendMsg(discordWebhookUrl, msg);

                    // 빌드 풀을 순회하며,
                    foreach (var buildInfo in buildInfos)
                        // 빌드 시작
                        Internal_Build(in buildInfo);
                    
                    // StackTrace Revert
                    SetLogTypes(StackTraceLogType.ScriptOnly);
                }

                if (!skipUpload)
                {
                    // 업로드 시작을 알림
                    if (sendDiscordMessage)
                    {
                        var uploadStartContent = $":arrow_heading_up: **스팀웍스 업로드를 시작합니다!** [{GetNowTime()}]\n";
                        foreach (var uploadInfo in uploadInfos)
                            uploadStartContent += $"- {uploadInfo.app.name}({uploadInfo.app.appID}) | 브랜치({uploadInfo.app.liveBranch})\n";
                        DiscordWebhook.SendMsg(discordWebhookUrl, uploadStartContent);
                    }
                    
                    // 업로드 시작
                    TerminalUtility.OpenTerminal(sdkPath, username, uploadInfos);
                
                    // 종료!
                    foreach (var uploadInfo in uploadInfos)
                    {
                        var logPath = Path.Combine(uploadInfo.logPath, $"app_build_{uploadInfo.app.appID}.log");
                        var lines = File.ReadAllLines(logPath);
                        // 공백 및 빈 줄 제거
                        var trimmedLines = lines.Where(line => !string.IsNullOrWhiteSpace(line)); 
                        // 하나의 문자열로 결합
                        var resultString = string.Join(Environment.NewLine, trimmedLines);
                    
                        var resultMsg = new JDiscordMsg {embeds = new JDiscordEmbed[1]};
                        resultMsg.embeds[0] = new JDiscordEmbed
                        {
                            title = $":star: 스팀웍스 업로드 성공: {uploadInfo.app.name}({uploadInfo.app.appID})\n" +
                                    $"https://partner.steamgames.com/apps/builds/{uploadInfo.app.appID}",
                            description = resultString
                        };
                        if(sendDiscordMessage)
                            DiscordWebhook.SendMsg(discordWebhookUrl, resultMsg);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                EditorApplication.Beep();
            }
            finally
            {
                // 원래의 빌드 프로필로 돌려주기
                BuildProfile.SetActiveBuildProfile(initialBuildProfile);
                
                // 스택트레이스 되돌리기
                SetLogTypes(StackTraceLogType.ScriptOnly);
            }
        }

        private void Internal_Build(in BuildInfo buildInfo)
        {
            var profile = buildInfo.depot.buildProfile;
            
            var option = new BuildPlayerWithProfileOptions
            {
                buildProfile = profile,
                locationPathName = buildInfo.buildPath,
                options = BuildOptions.CompressWithLz4HC
            };
            
            var report = BuildPipeline.BuildPlayer(option);
            if (report == null)
                throw new Exception($"({buildInfo.app.name}/{buildInfo.depot.buildTarget})빌드 실패 : 빌드 리포트가 null 입니다.");
            var summary = report.summary;
            
            // 에러 발생시,
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

                var embed = new JDiscordEmbed();
                embed.title = $":warning: 빌드 실패({buildInfo.app.name}/{buildInfo.depot.buildTarget})";
                embed.description = $"{summary.totalErrors}개의 에러가 발생했습니다.\n";
                foreach (var error in errors) 
                    embed.description += $"{error}\n";
                
                if(sendDiscordMessage)
                    DiscordWebhook.SendMsg(discordWebhookUrl, null, embed);
                throw new Exception(embed.title);
            }

            // 빌드 성공 메세지
            var totalTime = $"{summary.totalTime.Minutes}분 {summary.totalTime.Seconds}초";
            var msg = new JDiscordMsg {embeds = new JDiscordEmbed[1]};
            msg.embeds[0] = new JDiscordEmbed
            {
                title = $"\u2705 빌드 성공: {buildInfo.app.name}({buildInfo.app.appID})\n",
                description = $"- **버전**: v{Application.version}({buildInfo.versionNumber})\n" +
                              $"- **타겟**: {buildInfo.depot.buildTarget}\n" +
                              $"- **경로**: {summary.outputPath}\n" +
                              $"- **소요시간**: {totalTime}\n"
            };
            
            if(sendDiscordMessage)
                DiscordWebhook.SendMsg(discordWebhookUrl, msg);
            Debug.Log(msg);
        }

        private void SetLogTypes(StackTraceLogType stackTraceLogType)
        {
            for (var i = 0; i <= (int)LogType.Exception; i++) 
                PlayerSettings.SetStackTraceLogType((LogType)i, stackTraceLogType);
        }
        
        private string GetAppExecutionExtension(BuildTarget target)
        {
            if(target is BuildTarget.StandaloneOSX)
                return ".app";

            if(target is BuildTarget.StandaloneWindows or BuildTarget.StandaloneWindows64)
                return ".exe";
            
            if (target is BuildTarget.StandaloneLinux64)
                return ".x86_64";

            return ".exe";
        }

        private string GetNowTime() => $"{DateTime.Now:yyyy/MM/dd HH:mm:ss}";

        private void OnValidate()
        {
            appName = Application.productName;
            versionText = Application.version;
        }
    }
}