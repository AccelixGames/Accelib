using System;
using Accelib.Module.AccelNovel.Control.Parameter;
using Accelib.Module.AccelNovel.Control.Utility;
using Accelib.Module.AccelNovel.Model;
using Accelib.Module.AccelNovel.Model.SO;
using Accelib.Module.Audio;
using Accelix.Accelib.AccelNovel.Model;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Function
{
    public class ActionController_Scn : ActionController_Default
    {
        public override string Keyword => "scn";
        public override bool IsPlaying => true;
        public override bool CanSkip => false;

        [SerializeField] private ParameterController paramController;
        [SerializeField] private SO_NovelConfig config;

        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            var scnKey = action.value;

            try
            {
                // config 체크
                if (!config) throw new Exception("config가 null 입니다.");
                
                // 씬 키 체크
                if (string.IsNullOrEmpty(scnKey)) throw new Exception("씬 키가 비었습니다.");
            
                // 씬 로드
                var scn =  config.GetScenario(scnKey);
                if (!scn) throw new Exception($"시나리오({scnKey})를 찾을 수 없습니다.");

                // 세이브파일 저장
                var saveResult = SaveUtility.AutoSave(new SaveData
                {
                    scnKey = scn.ScnKey,
                    lineIndex = 0,
                    preview = CameraCaptureModule.Capture(),
                    
                    actionState = new SerializedDictionary<string, string>(),
                
                    affection = paramController.Affection.Value,
                    playerName = paramController.PlayerName.Value,
                    replay = paramController.Novel.ReplayCount
                }, this);
                SaveUtility.SetNewGameFlag(false);
                if (!saveResult) throw new Exception("저장에 실패했습니다.");
                
                // 노블 설정
                paramController.Novel.SetScnChange();
            
                // 소리 멈춤
                AudioSingleton.StopAllChannel(true);
                
                // 로딩
                // LoadingSystem.ChangeScene(config.DialogueScn, 1);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message, this);
            }
        }
    }
}