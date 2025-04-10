using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Accelib.Editor.Architecture;

namespace Accelib.Editor.AutoBuild.Steamwork.Control
{
    public class TerminalControl_OSX : TerminalControl
    {
        public override int OpenTerminal(string sdkPath, string username, in List<UploadInfo> uploadInfoList)
        {
            var steamCmdCommand  = $"{sdkPath}/tools/ContentBuilder/builder_osx/steamcmd.sh ";
            
            steamCmdCommand     += $"+login {username} ";
            foreach (var uploadInfo in uploadInfoList) 
                steamCmdCommand += $"+run_app_build_http \"{uploadInfo.vdfPath}\" ";
            steamCmdCommand +=     $"+quit";

            // 프로세스 실행
            Process.Start(new ProcessStartInfo
            {
                FileName = "/System/Applications/Utilities/Terminal.app",
                UseShellExecute = true
            });
            
            // 스팀 CMD 실행
            return RunSteamcmdCommand(steamCmdCommand);
        }

        public override int RunSteamcmdCommand(string steamcmdCommand)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash", // Bash를 사용해 steamcmd.sh 실행
                Arguments = $"-c \"{steamcmdCommand}\"", // -c 옵션을 사용하여 명령어 실행
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process();
            
            process.StartInfo = processStartInfo;
            process.OutputDataReceived += (sender, args) =>
            {
                var msg = args.Data;
                if (string.IsNullOrEmpty(msg)) return;
                    
                UnityEngine.Debug.Log(args.Data); // 명령 결과를 로그로 출력
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    UnityEngine.Debug.LogError(args.Data); // 오류 메시지를 로그로 출력
            };

            process.Start();
            process.BeginOutputReadLine(); // 비동기적으로 출력을 읽기 시작
            process.BeginErrorReadLine();   // 비동기적으로 오류 출력을 읽기 시작
            process.WaitForExit(); // 프로세스가 끝날 때까지 대기

            var exitCode = process.ExitCode;
            UnityEngine.Debug.Log($"steamcmd 프로세스가 종료되었습니다. 종료 코드: {exitCode}");
            return exitCode;
        }
    }
}