#if UNITY_STANDALONE && STEAMWORKS_NET && !DISABLESTEAMWORKS
using System;
using System.IO;
using Accelib.Logging;
using Accelib.Module.GameConfig;
using Steamworks;
using UnityEngine;

using App = HeathenEngineering.SteamworksIntegration.API.App;
using UserClient = HeathenEngineering.SteamworksIntegration.API.User.Client;

namespace Accelib.Module.SaveLoad.RemoteStorage
{
    public class RemoteStorage_Steam : RemoteStorage_Local
    {
        public override string Name => "Steam";
        
        public override string GetFilePath(string fileName)
        {
            // Path
            string path = null;

            try
            {
                if (SteamAPI.IsSteamRunning())
                {
                    // Initialize
                    if (!App.Initialized)
                        App.Client.Initialize(SteamConfig.Load().AppId);

                    if (App.Initialized)
                    {
                        var user = UserClient.Id;
                        var steamId = user.SteamId.ToString();
                        path = Path.Combine("Steam", steamId, fileName);
                    }
                    else
                    {
                        Deb.LogWarning("스팀 초기화 실패: " + App.InitializationErrorMessage);
                    }
                }
                else
                {
                    Deb.LogWarning("스팀 클라이언트가 실행중이지 않습니다. Local에 저장합니다.");
                }
            }
            catch (Exception e)
            {
                Deb.LogException(e);
            }
            finally
            {
                if(string.IsNullOrEmpty(path))
                    path = base.GetFilePath(fileName);
            }
            
            // Return Path
            return Path.Combine(Application.persistentDataPath, "SavesDir", path);
        }
    }
}
#endif