using System.Collections.Generic;
using System.Text.RegularExpressions;
using Accelib.Extensions;
using Accelib.Module.AccelNovel.Control.Resource;
using Accelib.Module.AccelNovel.Model;
using Accelib.Module.AccelNovel.Model.SO;
using Accelib.Module.Audio.Data;
using Accelib.Utility;
using Accelix.Accelib.AccelNovel.Model;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Dialogue
{
    public sealed class ActionController_Dialogue : ActionController_Default
    {
        public override string Keyword => "dlg";
        public override bool IsPlaying => dialogueModule.IsShowingText;

        [SerializeField] private SO_NovelConfig config;
        [SerializeField] private DialogueModule dialogueModule;
        [SerializeField] private StringVariable playerName;
        [SerializeField] private AudioRefSO voiceRef;

        [Header("Log")]
        [SerializeField, Range(1, 128)] private int maxLogCount = 64;
        [SerializeField] private List<LogData> logList;
        
        public override void Initialize()
        {
            dialogueModule.Initialize();
            logList.Clear();
        }

        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            var pName = playerName.Value.Omit(7);
            if (string.IsNullOrEmpty(playerName.Value)) pName = "올리";
            
            // 이름 설정
            var character = config.GetCharacter(action.characterKey);
            var characterName = character?.displayName?.Replace("{player}", pName);
            dialogueModule.SetNameTag(characterName);
            
            // 텍스트 재생
            var text = action.value.Replace("{player}", pName);
            text = text.Replace("{이/가}", KoreanTextHelper.GetSubjectParticle_I_Ga(pName));
            text = text.Replace("{이/-}", KoreanTextHelper.GetParticle(pName, "이", ""));
            text = text.Replace("{은/는}", KoreanTextHelper.GetTopicParticle_Eun_Neun(pName));
            text = text.Replace("{을/를}", KoreanTextHelper.GetObjectParticle_Eul_Reul(pName));
            text = text.Replace("{아/야}", KoreanTextHelper.GetParticle(pName, "아", "야"));
            dialogueModule.ShowText(text);
            
            // 보이스 재생
            AudioClip voiceClip = null;
            if (!string.IsNullOrEmpty(action.voiceKey))
                voiceClip = ResourceProvider.Instance.GetClip(action.voiceKey);
            
            // 보이스 재생
            if (voiceClip)
            {
                voiceRef.SetClip(voiceClip);
                voiceRef.Play();
            }
            // 보이스가 없으면, 스킵
            else
            {
                if(voiceRef.Clip)
                    voiceRef.Stop(true);
            }
            
            // 캐릭터 스프라이트
            // UnityEngine.Sprite sprite = null;
            // if (character != null)
            // {
            //     var spriteKey = character.resourceKeyDict.GetValueOrDefault("", null);
            //     if(!string.IsNullOrEmpty(spriteKey))
            //         sprite = ResourceProvider.Instance.GetSprite(spriteKey);
            // }
            
            // 로그 설정
            if(logList.Count >= maxLogCount)
                logList.RemoveAt(0);
            logList.Add(new LogData
            {
                name = characterName,
                text = RemoveBracketContents(text), 
                portrait = character?.thumbnail,
                voiceKey = action.voiceKey
            });
        }
        
        private string RemoveBracketContents(string input)
        {
            // <> 괄호 안 내용 제거
            var result = Regex.Replace(input, "<[^<>]*>", "");
            // [] 괄호 안 내용 제거
            result = Regex.Replace(result, @"\[[^\[\]]*\]", "");
            return result;
        }

        public override void ParseResources(ActionLine action, ref HashSet<string> sprites, ref HashSet<string> audios)
        {
            // 오디오
            if(!string.IsNullOrEmpty(action.voiceKey))
                audios.Add(action.voiceKey);
            
            // // 캐릭터 기본 이미지
            // var character = config.GetCharacter(action.characterKey);
            // if (character != null)
            // {
            //     var spriteKey = character.resourceKeyDict.GetValueOrDefault("", null);
            //     if(!string.IsNullOrEmpty(spriteKey))
            //         sprites.Add(spriteKey);   
            // }
        }

        public override void SkipAction(bool isFastForward = false)
        {
            dialogueModule.SkipText();
        }
    }
}