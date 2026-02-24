using System.Collections.Generic;
using System.Diagnostics;
using Accelib.Editor.Architecture;
using Accelib.Editor.AutoBuild.Steamwork.Control;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Accelib.Editor.AutoBuild.Steamwork
{
    /// <summary>플랫폼별 터미널 제어 유틸리티</summary>
    public static class TerminalUtility
    {
        /// <summary>SteamCMD로 빌드 업로드를 실행한다.</summary>
        public static int OpenTerminal(string sdkPath, string username, in List<UploadInfo> uploadInfoList)
        {
            var control = CreateControl();
            if (control == null) return -1;
            return control.OpenTerminal(sdkPath, username, uploadInfoList);
        }

        /// <summary>SteamCMD 로그인을 검증한다. 반환값 0 = 성공.</summary>
        public static int VerifyLogin(string sdkPath, string username)
        {
            var control = CreateControl();
            if (control == null) return -1;
            return control.VerifyLogin(sdkPath, username);
        }

        private static TerminalControl CreateControl()
        {
            var platform = Application.platform;
            if (platform == RuntimePlatform.WindowsEditor)
                return new TerminalControl_Windows();
            if (platform == RuntimePlatform.OSXEditor)
                return new TerminalControl_OSX();

            Debug.LogError($"Platform not supported: {platform}");
            return null;
        }
    }
}