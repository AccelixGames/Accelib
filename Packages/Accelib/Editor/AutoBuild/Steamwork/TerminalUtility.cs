using System.Collections.Generic;
using System.Diagnostics;
using Accelib.Editor.Architecture;

namespace Accelib.Editor.Steamwork
{
    public static class TerminalUtility
    {
        public static void OpenTerminalOSX(string sdkPath, string username, in List<string> appVdfPaths)
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
            RunSteamcmdCommand(steamCmdCommand);
        }
        
        private static void RunSteamcmdCommand(string steamcmdCommand)
        {
            // Run the SteamCMD command in Terminal
            var runCommandInfo = new ProcessStartInfo
            {
                FileName = "osascript", // osascript is a command-line tool for executing AppleScripts
                Arguments = $"-e 'tell application \"Terminal\" to do script \"{steamcmdCommand}\"'",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(runCommandInfo);
        }
    }
}