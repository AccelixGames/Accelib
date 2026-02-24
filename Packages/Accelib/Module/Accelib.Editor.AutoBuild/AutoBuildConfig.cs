using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Accelib.Editor.Architecture;
using Accelib.Editor.AutoBuild.Steamwork;
using Accelib.Editor.Steamwork;
using Accelib.Editor.Utility.Discord;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Profile;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Accelib.Editor
{
    /// <summary>Steam 빌드 자동화 설정</summary>
    [CreateAssetMenu(fileName = "AutoBuildConfig", menuName = "Accelib/AutoBuildConfig", order = 0)]
    public class AutoBuildConfig : ScriptableObject
    {
        private static string Key(string key) => $"Accelix.{nameof(AutoBuildConfig)}.{key}";

        [TitleGroup("#앱")]
        [SerializeField] private List<AppConfig> apps;
        
        [TitleGroup("#디스코드 메세지")]
        [SerializeField] private bool sendDiscordMessage = true;
        [TitleGroup("#디스코드 메세지"), EnableIf(nameof(sendDiscordMessage))]
        [SerializeField, TextArea] private string discordWebhookUrl =
            "https://discord.com/api/webhooks/1312969930349740043/LjGysdGxpCjvdJT7YPK8d95YF9Nq9zikqO2sytt2GW7khn6LTrAJoAGTa7QXQDv_Q5c4";

        
        [TitleGroup("#빌드 옵션", indent: true)]
        [InfoBox("계정명을 입력해주세요!", InfoMessageType.Error, visibleIfMemberName: nameof(IsUsernameInvalid))]
        [ShowInInspector, PropertyOrder(-1)]
        private string Username
        {
            get => EditorPrefs.GetString(Key(nameof(Username)));
            set => EditorPrefs.SetString(Key(nameof(Username)), value);
        }
        [TitleGroup("#빌드 옵션")]
        [InfoBox("SDK 경로가 존재하지 않습니다!", InfoMessageType.Error, visibleIfMemberName: nameof(IsSdkPathInvalid))]
        [InfoBox("steamcmd 실행 파일을 찾을 수 없습니다!", InfoMessageType.Error, visibleIfMemberName: nameof(IsSteamCmdMissing))]
        [FolderPath(AbsolutePath = true, RequireExistingPath = true)]
        [SerializeField] private string sdkPath = "C:/WorkSpace/github.com/steamwork-sdk";

        [TitleGroup("#빌드 옵션")]
        [OnValueChanged(nameof(BuildVersionText), InvokeOnInitialize = true, InvokeOnUndoRedo = true)]
        [SerializeField] private Vector3Int version = new(1, 0, 0);

        [TitleGroup("#빌드 옵션")]
        [SerializeField, ReadOnly] private string appName;

        [TitleGroup("#빌드 옵션")]
        [SerializeField, ReadOnly] private string versionText;

        [TitleGroup("#빌드 옵션")]
        [SerializeField, ReadOnly] private int versionNumber;

        [TitleGroup("#빌드 옵션")]
        [ShowInInspector, ReadOnly, PropertyOrder(10)]
        private string SteamCmdPath => GetSteamCmdPath() ?? "(미설정)";

        [TitleGroup("#빌드 옵션")]
        [ShowInInspector, ReadOnly, PropertyOrder(11)]
        [InlineButton(nameof(OpenUnityBuildPath), "열기")]
        private string UnityBuildPath => GetUnityBuildPath() ?? "(미설정)";

        [TitleGroup("#빌드 옵션")]
        [ShowInInspector, ReadOnly, PropertyOrder(12)]
        [InlineButton(nameof(OpenAddressablesSrcPath), "열기")]
        private string AddressablesSrcPath => GetAddressablesSrcPath();

        [TitleGroup("#빌드 옵션")]
        [ShowInInspector, ReadOnly, PropertyOrder(13)]
        [InlineButton(nameof(OpenAddressablesDstPath), "열기")]
        private string AddressablesDstPath => GetAddressablesDstPath();

        [TitleGroup("#빌드")]
        [SerializeField] private EAddressablesBuildMode addressablesBuildMode = EAddressablesBuildMode.Skip;
        [TitleGroup("#빌드")]
        [SerializeField] private bool skipBuild;
        [TitleGroup("#빌드")]
        [SerializeField] private bool skipUpload;
        [TitleGroup("#빌드")]
        [SerializeField, TextArea] private string patchNote;

#region Validation Properties

        private bool IsUsernameInvalid => string.IsNullOrEmpty(Username) || Username.Length <= 2;

        private bool IsSdkPathInvalid =>
            !skipUpload && (string.IsNullOrEmpty(sdkPath) || !Directory.Exists(sdkPath));

        private bool IsSteamCmdMissing
        {
            get
            {
                if (skipUpload || IsSdkPathInvalid) return false;
                var path = GetSteamCmdPath();
                return path != null && !File.Exists(path);
            }
        }

        #endregion

        #region Version

        private void BuildVersionText()
        {
            versionText = $"{version.x}.{version.y:D2}.{version.z:D2}";
            versionNumber = version.x * 10000 + version.y * 100 + version.z;

            PlayerSettings.bundleVersion = versionText;
            PlayerSettings.Android.bundleVersionCode = versionNumber;
            PlayerSettings.iOS.buildNumber = versionNumber.ToString();
            PlayerSettings.macOS.buildNumber = versionNumber.ToString();
        }

        #endregion

        #region Buttons

        [TitleGroup("#빌드 버튼")]
        [Button(DirtyOnClick = false, Name = "빌드")]
        public void StartBuildProcess()
        {
            // 초기 빌드 프로필 캐싱
            var initialBuildProfile = BuildProfile.GetActiveBuildProfile();

            try
            {
                // Phase 0: 사전 검증
                Validate_PreCheck();

                // Phase 1: 준비
                EditorSceneManager.SaveOpenScenes();
                BuildVersionText();
                SetLogTypes(StackTraceLogType.None);

                // Phase 2: VDF 스크립트 생성 + BuildInfo/UploadInfo 수집
                var basePath = Path.Combine(sdkPath, "tools", "ContentBuilder", "content");
                var buildInfos = new List<BuildInfo>();
                var uploadInfos = new List<UploadInfo>();

                appName = Application.productName;
                var note = string.IsNullOrEmpty(patchNote) ? "빈 패치노트" : patchNote;

                foreach (var app in apps)
                {
                    var appPath = Path.Combine(basePath, app.name);
                    var baseBuildPath = Path.Combine(appPath, "build");
                    var baseLogPath = Path.Combine(appPath, "log");
                    var scriptPath = Path.Combine(appPath, "scripts");

                    var patchNotePreInfo = $"[{app.name}({versionText})-";

                    // 디포를 순회하며 VDF 생성
                    for (var i = 0; i < app.depots.Count; i++)
                    {
                        var depot = app.depots[i];
                        if (!depot.includeInBuild) continue;

                        var platform = depot.buildTarget.ToString();
                        var buildPath = Path.Combine(baseBuildPath, platform, "v" + versionNumber);
                        var fileName = $"{appName}{GetAppExecutionExtension(depot.buildTarget)}";

                        // 디포 VDF 생성
                        var depotVdfPath = Path.Combine(scriptPath, $"depot_{depot.depotID}.vdf");
                        var depotContent = DepotUtility.GetDepotContent(appName, depot.depotID, buildPath);
                        DepotUtility.CreateFile(depotVdfPath, depotContent);

                        buildInfos.Add(new BuildInfo
                        {
                            app = app,
                            depot = depot,
                            versionStr = versionText,
                            versionNumber = versionNumber.ToString(),
                            buildPath = Path.Combine(buildPath, fileName)
                        });

                        patchNotePreInfo += $"{platform}";
                        if (i < app.depots.Count - 1)
                            patchNotePreInfo += ", ";
                    }

                    // 앱 VDF 생성
                    var appVdfPath = Path.Combine(scriptPath, $"app_{app.appID}.vdf");
                    var desc = $"{patchNotePreInfo}]\n" + note;
                    var logPath = Path.Combine(baseLogPath, "v" + versionNumber);
                    var appContent =
                        DepotUtility.GetAppContent(app.appID, desc, logPath, app.liveBranch, app.depots);
                    DepotUtility.CreateFile(appVdfPath, appContent);

                    uploadInfos.Add(new UploadInfo
                    {
                        app = app,
                        logPath = logPath,
                        vdfPath = appVdfPath
                    });
                }

                // 활성 디포 검증
                if (buildInfos.Count == 0)
                    throw new Exception("활성 디포가 없습니다. 빌드할 대상이 없습니다.");

                buildInfos.Sort((a, b) => a.depot.buildTarget.CompareTo(b.depot.buildTarget));
                var builderMsg = $"*{Username} | {versionText} | {note}*\n";

                if (!skipBuild)
                {
                    // Phase 3: Addressables 빌드
                    Internal_BuildAddressables();

                    // Phase 4: 플레이어 빌드
                    var buildStartMsg = $":computer: **빌드를 시작합니다!** [{GetNowTime()}]\n";
                    buildStartMsg += builderMsg;
                    foreach (var buildInfo in buildInfos)
                        buildStartMsg += $"- {buildInfo.app.name} | {buildInfo.depot.buildTarget}\n";
                    if (sendDiscordMessage)
                        DiscordWebhook.SendMsg(discordWebhookUrl, buildStartMsg);

                    // 각 디포 빌드 + Remote 복사 + 결과 알림
                    foreach (var buildInfo in buildInfos)
                    {
                        // 빌드 실행 (에러 시 throw)
                        var summary = Internal_Build(in buildInfo);

                        // Addressables Remote 콘텐츠 복사
                        var buildDir = Path.GetDirectoryName(buildInfo.buildPath);
                        var copyCount = Internal_CopyAddressablesRemote(buildDir, buildInfo.depot.buildTarget);

                        // 빌드 성공 Discord 알림
                        Internal_SendBuildSuccess(in buildInfo, in summary, copyCount);
                    }

                    SetLogTypes(StackTraceLogType.ScriptOnly);
                }

                Thread.Sleep(10);

                if (!skipUpload)
                {
                    // Phase 5: SteamCMD 업로드
                    if (sendDiscordMessage)
                    {
                        var uploadStartMsg =
                            $":arrow_heading_up: **스팀웍스 업로드를 시작합니다!** [{GetNowTime()}]\n";
                        uploadStartMsg += builderMsg;
                        foreach (var uploadInfo in uploadInfos)
                            uploadStartMsg +=
                                $"- {uploadInfo.app.name}({uploadInfo.app.appID}) | 브랜치({uploadInfo.app.liveBranch})\n";
                        DiscordWebhook.SendMsg(discordWebhookUrl, uploadStartMsg);
                    }

                    // 업로드 실행
                    TerminalUtility.OpenTerminal(sdkPath, Username, uploadInfos);

                    // 업로드 결과 Discord 알림
                    foreach (var uploadInfo in uploadInfos)
                    {
                        var logPath = Path.Combine(uploadInfo.logPath,
                            $"app_build_{uploadInfo.app.appID}.log");
                        var lines = File.ReadAllLines(logPath);
                        var trimmedLines = lines.Where(line => !string.IsNullOrWhiteSpace(line));
                        var resultString = string.Join(Environment.NewLine, trimmedLines);

                        var resultMsg = new JDiscordMsg { embeds = new JDiscordEmbed[1] };
                        resultMsg.embeds[0] = new JDiscordEmbed
                        {
                            title =
                                $":star: 스팀웍스 업로드 성공: {uploadInfo.app.name}({uploadInfo.app.appID})\n" +
                                $"https://partner.steamgames.com/apps/builds/{uploadInfo.app.appID}",
                            description = resultString
                        };
                        if (sendDiscordMessage)
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
                // 원래의 빌드 프로필로 복원
                BuildProfile.SetActiveBuildProfile(initialBuildProfile);

                // 스택트레이스 복원
                SetLogTypes(StackTraceLogType.ScriptOnly);
            }
        }

        // [TitleGroup("#빌드 버튼")]
        // [Button(DirtyOnClick = false, Name = "SteamCMD 열기(첫 로그인용)")]
        // public void StartSteamCmd()
        // {
        //     var path = GetSteamCmdPath();
        //     if (string.IsNullOrEmpty(path) || !File.Exists(path))
        //     {
        //         Debug.LogError($"steamcmd를 찾을 수 없습니다: {path}");
        //         return;
        //     }
        //
        //     EditorUtility.OpenWithDefaultApp(path);
        // }

        [TitleGroup("#빌드 버튼")]
        [Button(DirtyOnClick = false, Name = "SteamCMD 로그인 테스트")]
        public void TestSteamCmdLogin()
        {
            if (IsUsernameInvalid)
            {
                Debug.LogError("계정명을 먼저 입력해주세요.");
                return;
            }

            var path = GetSteamCmdPath();
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                Debug.LogError($"steamcmd를 찾을 수 없습니다: {path}");
                return;
            }

            Debug.Log("SteamCMD 로그인 테스트 중...");
            var result = TerminalUtility.VerifyLogin(sdkPath, Username);
            if (result == 0)
                Debug.Log("SteamCMD 로그인 성공!");
            else
                Debug.LogError($"SteamCMD 로그인 실패 (종료 코드: {result})");
        }

        #endregion

        #region Validation

        /// <summary>빌드 시작 전 사전 검증을 수행한다.</summary>
        private void Validate_PreCheck()
        {
            // Discord URL 검증
            if (sendDiscordMessage && string.IsNullOrEmpty(discordWebhookUrl))
                throw new Exception("DiscordWebhookUrl is empty");

            // 앱 설정 검증
            if (apps == null || apps.Count == 0)
                throw new Exception("앱이 설정되지 않았습니다.");

            // 업로드 관련 검증
            if (!skipUpload)
            {
                // SDK 경로 검증
                if (string.IsNullOrEmpty(sdkPath) || !Directory.Exists(sdkPath))
                    throw new Exception($"SDK 경로가 존재하지 않습니다: {sdkPath}");

                // SteamCMD 실행파일 검증
                var cmdPath = GetSteamCmdPath();
                if (string.IsNullOrEmpty(cmdPath) || !File.Exists(cmdPath))
                    throw new Exception($"steamcmd를 찾을 수 없습니다: {cmdPath}");

                // Username 검증
                if (IsUsernameInvalid)
                    throw new Exception("올바르지 않은 Username: " + Username);

                // SteamCMD 로그인 검증
                Debug.Log("SteamCMD 로그인 검증 중...");
                var loginResult = TerminalUtility.VerifyLogin(sdkPath, Username);
                if (loginResult != 0)
                    throw new Exception(
                        $"SteamCMD 로그인 실패 (종료 코드: {loginResult}). 먼저 'SteamCMD 열기' 버튼으로 로그인해주세요.");
                Debug.Log("SteamCMD 로그인 검증 성공!");
            }
        }

        #endregion

        #region Internal — Addressables

        /// <summary>Addressables 빌드를 수행한다.</summary>
        private void Internal_BuildAddressables()
        {
            if (addressablesBuildMode == EAddressablesBuildMode.Skip)
            {
                Debug.Log("Addressables 빌드 건너뜀.");
                return;
            }

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
                throw new Exception("AddressableAssetSettings를 찾을 수 없습니다.");

            if (sendDiscordMessage)
                DiscordWebhook.SendMsg(discordWebhookUrl,
                    $":package: **Addressables 빌드 시작!** [{GetNowTime()}] ({addressablesBuildMode})");

            switch (addressablesBuildMode)
            {
                case EAddressablesBuildMode.CleanBuild:
                {
                    Debug.Log("Addressables 클린 빌드 시작...");
                    AddressableAssetSettings.BuildPlayerContent(out var result);
                    if (!string.IsNullOrEmpty(result?.Error))
                        throw new Exception($"Addressables 클린 빌드 실패: {result.Error}");
                    Debug.Log($"Addressables 클린 빌드 성공! ({result.Duration:F1}초)");
                    break;
                }
                case EAddressablesBuildMode.ContentUpdate:
                {
                    Debug.Log("Addressables 콘텐츠 업데이트 시작...");

                    // Content state 파일 경로 확인
                    var contentStatePath = ContentUpdateScript.GetContentStateDataPath(false);
                    if (string.IsNullOrEmpty(contentStatePath) || !File.Exists(contentStatePath))
                        throw new Exception(
                            $"Content state 파일을 찾을 수 없습니다: {contentStatePath}\n클린 빌드를 먼저 실행해주세요.");

                    var updateResult = ContentUpdateScript.BuildContentUpdate(settings, contentStatePath);
                    if (!string.IsNullOrEmpty(updateResult?.Error))
                        throw new Exception($"Addressables 콘텐츠 업데이트 실패: {updateResult.Error}");
                    Debug.Log($"Addressables 콘텐츠 업데이트 성공! ({updateResult.Duration:F1}초)");
                    break;
                }
            }

            if (sendDiscordMessage)
                DiscordWebhook.SendMsg(discordWebhookUrl,
                    $":white_check_mark: **Addressables 빌드 완료!** [{GetNowTime()}]");
        }

        /// <summary>Addressables Remote 콘텐츠를 빌드 출력 폴더로 복사한다.</summary>
        /// <returns>복사된 파일 수</returns>
        private int Internal_CopyAddressablesRemote(string buildOutputDir, BuildTarget buildTarget)
        {
            if (addressablesBuildMode == EAddressablesBuildMode.Skip)
                return 0;

            var projectRoot = Path.GetDirectoryName(Application.dataPath);
            var targetName = buildTarget.ToString();
            var remoteSrcBase = Path.Combine(projectRoot, "Remote");

            if (!Directory.Exists(remoteSrcBase))
            {
                Debug.LogWarning($"Remote 폴더가 존재하지 않습니다: {remoteSrcBase}");
                return 0;
            }

            // 정확한 BuildTarget 폴더 탐색
            var remoteSrcPath = Path.Combine(remoteSrcBase, targetName);
            if (!Directory.Exists(remoteSrcPath))
            {
                // StandaloneWindows ↔ StandaloneWindows64 등 유사 이름 탐색
                var baseName = targetName.Replace("64", "");
                var dirs = Directory.GetDirectories(remoteSrcBase);
                remoteSrcPath = dirs.FirstOrDefault(d =>
                {
                    var name = Path.GetFileName(d);
                    return name != null && name.StartsWith(baseName);
                });

                if (remoteSrcPath == null)
                {
                    Debug.LogWarning($"Remote/{targetName} 폴더를 찾을 수 없습니다.");
                    return 0;
                }
            }

            // 대상 폴더에 복사
            var folderName = Path.GetFileName(remoteSrcPath);
            var remoteDstPath = Path.Combine(buildOutputDir, "Remote", folderName);
            var copyCount = CopyDirectoryRecursive(remoteSrcPath, remoteDstPath);

            Debug.Log($"Addressables Remote 복사 완료: {copyCount}개 파일 → {remoteDstPath}");
            return copyCount;
        }

        #endregion

        #region Internal — Build

        /// <summary>플레이어 빌드를 수행한다. 에러 시 Discord 알림 후 throw.</summary>
        /// <returns>빌드 성공 시 BuildSummary</returns>
        private BuildSummary Internal_Build(in BuildInfo buildInfo)
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
                throw new Exception(
                    $"({buildInfo.app.name}/{buildInfo.depot.buildTarget})빌드 실패 : 빌드 리포트가 null 입니다.");

            var summary = report.summary;

            // 에러 발생시 Discord 알림 후 throw
            if (summary.totalErrors > 0)
            {
                var errors = new List<string>();

                foreach (var step in report.steps)
                {
                    var err =
                        $"[{step.name}({step.depth} : {Mathf.RoundToInt((float)step.duration.TotalSeconds)}sec)]";
                    foreach (var stepMsg in step.messages)
                        if (stepMsg.type is LogType.Assert or LogType.Error or LogType.Exception)
                            errors.Add($"{err}{stepMsg.type} : {stepMsg.content}");
                }

                var embed = new JDiscordEmbed
                {
                    title = $":warning: 빌드 실패({buildInfo.app.name}/{buildInfo.depot.buildTarget})",
                    description = $"{summary.totalErrors}개의 에러가 발생했습니다.\n"
                };
                foreach (var error in errors)
                    embed.description += $"{error}\n";

                if (sendDiscordMessage)
                    DiscordWebhook.SendMsg(discordWebhookUrl, null, embed);
                throw new Exception(embed.title);
            }

            return summary;
        }

        /// <summary>빌드 성공 Discord 알림을 전송한다.</summary>
        private void Internal_SendBuildSuccess(in BuildInfo buildInfo, in BuildSummary summary, int copyCount)
        {
            var totalTime = $"{summary.totalTime.Minutes}분 {summary.totalTime.Seconds}초";
            var desc = $"- **버전**: v{Application.version}({buildInfo.versionNumber})\n" +
                       $"- **타겟**: {buildInfo.depot.buildTarget}\n" +
                       $"- **경로**: {summary.outputPath}\n" +
                       $"- **소요시간**: {totalTime}\n";

            if (copyCount > 0)
                desc += $"- **Addressables**: {copyCount}개 파일 복사 완료\n";

            var msg = new JDiscordMsg { embeds = new JDiscordEmbed[1] };
            msg.embeds[0] = new JDiscordEmbed
            {
                title = $"\u2705 빌드 성공: {buildInfo.app.name}({buildInfo.app.appID})",
                description = desc
            };

            if (sendDiscordMessage)
                DiscordWebhook.SendMsg(discordWebhookUrl, msg);
            Debug.Log($"빌드 성공: {buildInfo.app.name}/{buildInfo.depot.buildTarget} ({totalTime})");
        }

        #endregion

        #region Helpers

        private string GetSteamCmdPath()
        {
            if (string.IsNullOrEmpty(sdkPath)) return null;
            return Application.platform switch
            {
                RuntimePlatform.WindowsEditor => Path.Combine(sdkPath, "tools", "ContentBuilder", "builder",
                    "steamcmd.exe"),
                RuntimePlatform.OSXEditor => Path.Combine(sdkPath, "tools", "ContentBuilder", "builder_osx",
                    "steamcmd.sh"),
                _ => null
            };
        }

        private string GetUnityBuildPath()
        {
            if (string.IsNullOrEmpty(sdkPath) || apps == null || apps.Count == 0)
                return null;

            var app = apps[0];
            if (app.depots == null || app.depots.Count == 0)
                return null;

            var depot = app.depots.FirstOrDefault(d => d.includeInBuild);
            if (depot == null) return null;

            var basePath = Path.Combine(sdkPath, "tools", "ContentBuilder", "content");
            var platform = depot.buildTarget.ToString();
            return Path.Combine(basePath, app.name, "build", platform, $"v{versionNumber}");
        }

        private string GetAddressablesSrcPath()
        {
            var projectRoot = Path.GetDirectoryName(Application.dataPath);
            var target = EditorUserBuildSettings.activeBuildTarget.ToString();
            return Path.Combine(projectRoot, "Remote", target);
        }

        private string GetAddressablesDstPath()
        {
            var buildPath = GetUnityBuildPath();
            if (string.IsNullOrEmpty(buildPath)) return "(미설정)";

            var target = EditorUserBuildSettings.activeBuildTarget.ToString();
            return Path.Combine(buildPath, "Remote", target);
        }

        private static int CopyDirectoryRecursive(string srcDir, string dstDir)
        {
            if (!Directory.Exists(dstDir))
                Directory.CreateDirectory(dstDir);

            var count = 0;

            // 파일 복사
            foreach (var file in Directory.GetFiles(srcDir))
            {
                var destFile = Path.Combine(dstDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
                count++;
            }

            // 하위 폴더 재귀 복사
            foreach (var dir in Directory.GetDirectories(srcDir))
            {
                var destSubDir = Path.Combine(dstDir, Path.GetFileName(dir));
                count += CopyDirectoryRecursive(dir, destSubDir);
            }

            return count;
        }

        private void SetLogTypes(StackTraceLogType stackTraceLogType)
        {
            for (var i = 0; i <= (int)LogType.Exception; i++)
                PlayerSettings.SetStackTraceLogType((LogType)i, stackTraceLogType);
        }

        private static string GetAppExecutionExtension(BuildTarget target)
        {
            if (target is BuildTarget.StandaloneOSX)
                return ".app";
            if (target is BuildTarget.StandaloneWindows or BuildTarget.StandaloneWindows64)
                return ".exe";
            if (target is BuildTarget.StandaloneLinux64)
                return ".x86_64";
            return ".exe";
        }

        /// <summary>현재 시간을 포맷된 문자열로 반환한다.</summary>
        public static string GetNowTime() => $"{DateTime.Now:yyyy/MM/dd HH:mm:ss}";

        #endregion

        #region Folder Open

        private void OpenUnityBuildPath() => RevealFolder(GetUnityBuildPath());
        private void OpenAddressablesSrcPath() => RevealFolder(GetAddressablesSrcPath());
        private void OpenAddressablesDstPath() => RevealFolder(GetAddressablesDstPath());

        private static void RevealFolder(string path)
        {
            if (string.IsNullOrEmpty(path) || path.StartsWith("("))
            {
                Debug.LogWarning("경로가 설정되지 않았습니다.");
                return;
            }

            if (!Directory.Exists(path))
            {
                Debug.LogWarning($"폴더가 존재하지 않습니다: {path}");
                return;
            }

            EditorUtility.RevealInFinder(path);
        }

        #endregion

        #region Unity Callbacks

        private void OnValidate()
        {
            appName = Application.productName;
            versionText = Application.version;
        }

        #endregion
    }
}
