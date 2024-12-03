using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Accelib.Editor.Architecture;
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
                if (string.IsNullOrEmpty(sdkPath))
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
                var buildInfoList = new List<BuildInfo>();
                var appVdfList = new List<string>();

                // 앱을 순회하며,
                appName = Application.productName;
                foreach (var app in apps)
                {
                    // 앱 경로 구하기
                    var appPath = Path.Combine(basePath, app.name);
                    var baseBuildPath = Path.Combine(appPath, "build");
                    var baseLogPath = Path.Combine(appPath, "log");
                    var scriptPath = Path.Combine(appPath, "scripts");
                    
                    // 앱 빌드 스크립트 생성
                    var appVdfPath = Path.Combine(scriptPath, $"app_{app.appID}.vdf");
                    var appContent = DepotUtility.GetAppContent(app.appID, patchNote, baseBuildPath, 
                        baseLogPath, app.liveBranch, app.depots);
                    DepotUtility.CreateFile(appVdfPath, appContent);
                    
                    // 업로드 정보 추가
                    appVdfList.Add( appVdfPath);
                    
                    // 디포를 순회하며,
                    foreach (var depot in app.depots)
                    {
                        if (!depot.includeInBuild) continue;

                        // Platform
                        var platform = depot.buildTarget.ToString();
                        var subBuildPath = Path.Combine(platform, "v" + versionNumber);
                        var fileName = $"{appName}{GetAppExecutionExtension(depot.buildTarget)}";

                        // 디포 빌드 스크립트 생성
                        var depotVdfPath = Path.Combine(scriptPath, $"depot_{depot.depotID}.vdf");
                        var depotContent = DepotUtility.GetDepotContent(appName, depot.depotID, subBuildPath);
                        DepotUtility.CreateFile(depotVdfPath, depotContent);
                        
                        // 빌드 정보 추가
                        buildInfoList.Add(new BuildInfo
                        {
                            app = app,
                            depot = depot,
                            versionStr = versionStr,
                            versionNumber = versionNumber.ToString(),

                            buildPath = Path.Combine(baseBuildPath, subBuildPath, fileName),
                            logPath = Path.Combine(baseLogPath, platform, "v" + versionNumber),
                            scriptPath = scriptPath,
                        });
                    }
                }

                // 빌드 풀이 없으면 에러
                if (buildInfoList.Count == 0)
                    throw new Exception("No build pool found");

                // 플랫폼으로 빌드 풀 정렬
                buildInfoList.Sort((a, b) => a.depot.buildTarget.CompareTo(b.depot.buildTarget));

                // 디스코드 메세지 생성
                var msg = $":computer: **빌드를 시작합니다!** [{DateTime.Now:yyyy/dd/MM HH:mm:ss}]\n";
                foreach (var buildInfo in buildInfoList)
                    msg += $"- {buildInfo.app.name} | {buildInfo.depot.buildTarget} | {buildInfo.versionStr}\n";
                DiscordWebhook.SendMsg(discordWebhookUrl, msg);

                // 빌드 풀을 순회하며,
                foreach (var buildInfo in buildInfoList)
                    // 빌드 시작
                    Internal_Build(in buildInfo);
                
                // 업로드 시작
                TerminalUtility.OpenTerminalOSX(sdkPath, username, appVdfList);
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
                var embed = new JDiscordEmbed
                {
                    title = $":warning: 빌드 실패({buildInfo.app.name}/{buildInfo.depot.buildTarget})",
                    description = $"{summary.totalErrors}개의 에러가 발생했습니다.",
                };
                DiscordWebhook.SendMsg(discordWebhookUrl, null, embed);
                throw new Exception(embed.title);
            }

            // 빌드 성공 메세지
            var totalTime = $"{summary.totalTime.Minutes}분 {summary.totalTime.Seconds}초";
            var totalSize = summary.totalSize / ((ulong)8 * 1024 * 1024);
            var msg = new JDiscordMsg {embeds = new JDiscordEmbed[1]};
            msg.embeds[0] = new JDiscordEmbed
            {
                title = $"\u2705 빌드 성공: {buildInfo.app.name}({buildInfo.app.appID})\n",
                description = $"- **버전**: v{Application.version}({buildInfo.versionNumber})\n" +
                              $"- **타겟**: {buildInfo.depot.buildTarget}\n" +
                              $"- **경로**: {summary.outputPath}\n" +
                              $"- **소요시간**: {totalTime}\n" +
                              $"- **사이즈**: {totalSize}MB\n"
            };

            DiscordWebhook.SendMsg(discordWebhookUrl, msg);
        }

        private void Internal_Upload(in BuildInfo buildInfo)
        {
            
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

            return ".exe";
        }

        private void OnValidate()
        {
            appName = Application.productName;
            versionText = Application.version;
        }
    }
}