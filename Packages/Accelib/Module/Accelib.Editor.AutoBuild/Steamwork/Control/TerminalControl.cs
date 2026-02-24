using System.Collections.Generic;
using Accelib.Editor.Architecture;

namespace Accelib.Editor.AutoBuild.Steamwork.Control
{
    /// <summary>터미널을 통한 SteamCMD 실행 제어 기반 클래스</summary>
    public abstract class TerminalControl
    {
        /// <summary>SteamCMD로 빌드 업로드를 실행한다.</summary>
        public abstract int OpenTerminal(string sdkPath, string username, in List<UploadInfo> uploadInfoList);

        /// <summary>SteamCMD 명령어를 실행한다.</summary>
        public abstract int RunSteamcmdCommand(string steamcmdCommand);

        /// <summary>SteamCMD 로그인을 검증한다. 반환값 0 = 성공.</summary>
        public abstract int VerifyLogin(string sdkPath, string username);
    }
}