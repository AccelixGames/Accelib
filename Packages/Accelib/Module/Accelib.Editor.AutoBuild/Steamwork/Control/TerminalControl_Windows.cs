using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Accelib.Editor.Architecture;

namespace Accelib.Editor.AutoBuild.Steamwork.Control
{
    public class TerminalControl_Windows : TerminalControl
    {
        public override int OpenTerminal(string sdkPath, string username, in List<UploadInfo> uploadInfoList)
        {
            var steamCmdCommand = $"{sdkPath}/tools/ContentBuilder/builder/steamcmd.exe ";

            // SteamCMD 명령어 문자열 구성
            steamCmdCommand += $"+login {username} ";
            foreach (var uploadInfo in uploadInfoList)
                steamCmdCommand += $"+run_app_build_http \"{uploadInfo.vdfPath}\" ";
            steamCmdCommand += "+quit";

            return RunSteamcmdCommand(steamCmdCommand);
        }

        public override int RunSteamcmdCommand(string steamcmdCommand)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/C {steamcmdCommand}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process();

            process.StartInfo = processStartInfo;
            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    UnityEngine.Debug.Log(args.Data);
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    UnityEngine.Debug.LogError(args.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            var exitCode = process.ExitCode;
            UnityEngine.Debug.Log($"steamcmd 프로세스가 종료되었습니다. 종료 코드: {exitCode}");
            return exitCode;
        }

        /// <inheritdoc/>
        public override int VerifyLogin(string sdkPath, string username)
        {
            var steamCmdPath = Path.Combine(sdkPath, "tools", "ContentBuilder", "builder", "steamcmd.exe");
            return RunSteamcmdCommand($"{steamCmdPath} +login {username} +quit");
        }
    }
}