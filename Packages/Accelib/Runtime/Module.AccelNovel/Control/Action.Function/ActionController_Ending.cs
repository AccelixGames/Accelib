using System.Collections.Generic;
using Accelib.Logging;
using Accelib.Module.AccelNovel.Implements.SaveLoad.Collective;
using Accelib.Module.AccelNovel.Model;
using Accelib.Module.AccelNovel.View.Ending;
using Accelib.Module.SaveLoad;
using Accelix.Accelib.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model.Enum;
using Accelix.Singleton.SaveLoad.Statistics;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Function
{
    public class ActionController_Ending : ActionController_Default
    {
        public override string Keyword => "ending";
        public override bool IsPlaying => false;
        public override bool CanSkip => false;
        
        [Header("엔딩 오브젝트")]
        [SerializeField] private GameObject endingCanvas;
        [SerializeField] private Ending_Credit credit;
        [SerializeField] private Ending_Card card;
        [SerializeField] private Ending_VoiceCard voiceCard;
        [SerializeField] private EndingCreditResourceHandler resourceHandler;

        [Header("엔딩 정보")]
        [SerializeField] private SerializedDictionary<string, EndingData> endings;
        [SerializeField, TextArea] private string unlockedText = "새로운 엔딩이 해금되었습니다!";
        
        [Header("Next Scene")]
        [SerializeField, Scene] private string nextScene;
        
        public override void Initialize()
        {
            endingCanvas.SetActive(false);
            
            credit.gameObject.SetActive(false);
            card.gameObject.SetActive(false);
            voiceCard.gameObject.SetActive(false);
        }

        public override async void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            // 캐싱
            var collectiveHolder = SaveLoadSingleton.Get<SaveDataHolder_Collective>();
            var endingKey = action.value;
            
            // 리소스 로드
            await LoadAssets();
            
            // 지원되는 엔딩이라면,
            if (endings.TryGetValue(endingKey, out var ending))
            {
                // 플레이 횟수 추가
                if (ending.replayCountUp)
                {
                    var stats = SaveLoadSingleton.Get<SaveDataHolder_Stats>();
                    if (!stats) Deb.LogError("Failed to get stats from SaveDataHolder", this);
                    else stats.AddReplayCount();   
                }
                
                // 엔딩 컬렉션 해금.
                collectiveHolder.TryUnlock(ending.collective.AssetKey, true, unlockedText);
                
                // 앤딩 캔버스 활성화
                endingCanvas.SetActive(true);

                switch (ending.endingType)
                {
                    // 엔딩 크레딧 : 기본
                    case EEndingType.Credit:
                        credit.Open(ending, false);
                        break;
                    // 엔딩 카드
                    case EEndingType.Card:
                        card.Open(ending);
                        break;
                    // 얀데레 엔딩
                    case EEndingType.VoiceCard:
                        OpenVoiceCard(ending).Forget();
                        break;
                }
            }
        }

        private async UniTaskVoid OpenVoiceCard(EndingData data)
        {
            await voiceCard.Open();
            credit.Open(data, true);
        }

        public void Close()
        {
            Novel.Release();
            // LoadingSystem.ChangeScene(nextScene);
        }

        private async UniTask LoadAssets()
        {
            // bgm
            var keys = new List<string>();
            foreach (var data in endings.Values)
            {
                var key = data?.bgmKey;
                
                if(!string.IsNullOrEmpty(key) && !string.IsNullOrWhiteSpace(key))
                    keys.Add(key);
            }

            // cg
            var saveData = NovelController.GetCurrentSaveData_Static();
            var cgKeys = saveData?.cgKeyList ?? new List<string>();
             
            // 키 등록
            resourceHandler.SetResourceKey(cgKeys, keys);
            
            // 로드
            await resourceHandler.LoadResource();
        }
    }
}