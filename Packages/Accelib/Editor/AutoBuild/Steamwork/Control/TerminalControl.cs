using System.Collections.Generic;
using Accelib.Editor.Architecture;

namespace Accelib.Editor.AutoBuild.Steamwork.Control
{
    public abstract class TerminalControl
    {
        public abstract int OpenTerminal(string sdkPath, string username, in List<UploadInfo> uploadInfoList);

        public abstract int RunSteamcmdCommand(string steamcmdCommand);
    }
}