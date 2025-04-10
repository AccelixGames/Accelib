using System.Collections.Generic;
using System.Diagnostics;
using Accelib.Editor.Architecture;
using Accelib.Editor.AutoBuild.Steamwork.Control;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Accelib.Editor.AutoBuild.Steamwork
{
    public static class TerminalUtility
    {
        public static int OpenTerminal(string sdkPath, string username, in List<UploadInfo> uploadInfoList)
        {
            var platform = Application.platform;
            TerminalControl control;
            
            if (platform == RuntimePlatform.WindowsEditor)
                control = new TerminalControl_Windows();
            else if (platform == RuntimePlatform.OSXEditor)
                control = new TerminalControl_OSX();
            else
            {
                Debug.LogError($"Platform not supported: {platform}");
                return -1;  
            }
            
            return control.OpenTerminal(sdkPath, username, uploadInfoList);
        }
    }
}