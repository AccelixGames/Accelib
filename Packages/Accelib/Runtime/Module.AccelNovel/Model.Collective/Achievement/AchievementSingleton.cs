using System;
using Accelib.Core;
using Accelib.Logging;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_STANDALONE && STEAMWORKS_NET && !DISABLESTEAMWORKS
using Accelib.Module.GameConfig;
using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.API;
using Steamworks;
#elif UNITY_ANDROID || UNITY_IOS
using VoxelBusters.EssentialKit;
#endif

namespace Accelib.Module.AccelNovel.Model.Collective.Achievement
{
    public class AchievementSingleton : MonoSingleton<AchievementSingleton>
    {
        [field: Header("도전과제 전체 성공")]
        [field: SerializeField, ReadOnly] public bool IsAllAchieved { get; private set; }

        private void Start()
        {
            IsAllAchieved = false;
        }

        public bool CheckAndAchieve(string key)
        {
            try
            {
#if UNITY_STANDALONE && STEAMWORKS_NET && !DISABLESTEAMWORKS
            if (DemoHandler.IsDemo) return false;
            return SteamAchieve(key);
#elif UNITY_ANDROID || UNITY_IOS
                // Deb.Log($"Achiv Check! : {key} | IsAvailable({GameServices.IsAvailable()}) | IsAuth({GameServices.IsAuthenticated})");
                if (GameServices.IsAvailable() && GameServices.IsAuthenticated)
                    GameServices.ReportAchievementProgress(key, 100, (success, error) =>
                    {
                        if (!success)
                            Deb.LogError("Request to submit progress failed with error. Error: " + error, this);
                    });
#elif STOVE_BUILD
                StoveAchieve(key).Forget();
                return true;
#endif
            }
            catch (Exception e)
            {
                Deb.LogException(e, this);
            }

            return false;
        }

#if UNITY_STANDALONE && STEAMWORKS_NET && !DISABLESTEAMWORKS
        private bool SteamAchieve(string key)
        {
            try
            {
                if (SteamAPI.IsSteamRunning())
                {
                    // Initialize
                    if (!App.Initialized)
                        App.Client.Initialize(SteamConfig.Load().AppId);

                    if (App.Initialized)
                    {
                        AchievementData myAch = key;
                        if (myAch.IsAchieved)
                        {
                            Deb.Log("이미 달성된 과제입니다: " + key);
                            return true;
                        }

                        myAch.IsAchieved = true;
                        myAch.Store();
                        Deb.Log("도전과제 달성: " + key);
                        return true;
                    }

                    Deb.LogWarning("스팀 초기화 실패: " + App.InitializationErrorMessage);
                    return false;
                }

                Deb.LogWarning("스팀 클라이언트가 실행중이지 않습니다." + key);
                return false;
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                return false;
            }
        }
#elif STOVE_BUILD
        private async UniTaskVoid StoveAchieve(string key)
        {
            var res = await Stove.StoveGameSDK.ModifyStat(key, 1);;
            var result = "UpdatedFailed";
            if (res != null) result = res?.updated.ToString();

            AllAchievedCheck().Forget();
            // Deb.Log($"[도전과제]({key}:{res?.currentValue ?? 1}){result}");
        }
        
        public async UniTask AllAchievedCheck()
        {
            var achvStr = "ACHIEVED";
            var allAchievements = await Stove.StoveGameSDK.GetAllAchievement();
            if (allAchievements == null)
            {
                IsAllAchieved = false;
                Deb.LogError("[모든 도전과제] 가져오기 실패");
                return;
            }
            
            // 전체 도전과제 확인
            foreach (var achv in allAchievements)
                if (achv.status != achvStr)
                {
                    Deb.Log($"[모든 도전과제] {achv.name}가 아직 달성되지 않았습니다.");
                    IsAllAchieved = false;
                    return;
                }
            
            // 달성!!
            IsAllAchieved = true;
            Deb.Log($"[모든 도전과제] 달성 완료!!");
        }
#endif
    }
}