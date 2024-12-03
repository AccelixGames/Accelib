using System.Collections.Generic;
using System.Diagnostics;

namespace Accelib.Editor.Steamwork
{
    public static class TerminalUtility
    {
        public static int OpenTerminalOSX(string sdkPath, string username, in List<string> appVdfPaths)
        {
            var steamCmdCommand  = $"{sdkPath}/tools/ContentBuilder/builder_osx/steamcmd.sh ";
            steamCmdCommand     += $"+login {username} ";
            foreach (var appVdfPath in appVdfPaths) 
                steamCmdCommand += $"+run_app_build_http {appVdfPath} ";
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
        
        private static int RunSteamcmdCommand(string steamcmdCommand)
        {
            // Run the SteamCMD command in Terminal
            var runCommandInfo = new ProcessStartInfo
            {
                FileName = "osascript", // osascript is a command-line tool for executing AppleScripts
                Arguments = $"-e 'tell application \"Terminal\" to do script \"{steamcmdCommand}\"'",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // 프로세스 시작 및 종료 대기
            using var process = Process.Start(runCommandInfo);
            if (process != null)
            {
                // 프로세스가 종료될 때까지 대기
                process.WaitForExit();
                
                // 종료 코드 확인 가능
                var exitCode = process.ExitCode; 
                UnityEngine.Debug.Log($"프로세스가 종료되었습니다. 종료 코드: {exitCode}");
                return exitCode;
            }

            return -999;
        }
    }
}