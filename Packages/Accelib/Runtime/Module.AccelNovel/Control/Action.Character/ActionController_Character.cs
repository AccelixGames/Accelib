using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Extensions;
using Accelib.Logging;
using Accelib.Module.AccelNovel.Control.Resource;
using Accelib.Module.AccelNovel.Control.Utility;
using Accelib.Module.AccelNovel.Model;
using Accelib.Module.AccelNovel.Model.SO;
using Accelix.Accelib.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model.Enum;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Character
{
    public sealed class ActionController_Character : ActionControllerT<ActionController_Character.Data>
    {
        [Serializable]
        public class Data
        {
            public List<CharacterUnit.Data> unitDataList;
        }
        
        public override string Keyword => "char";
        public override bool IsPlaying => nextActionMode != ENextActionMode.Skip && isPlaying;
        
        [Header("Config")]
        [SerializeField] private SO_NovelConfig config;
        [SerializeField] private List<CharacterUnit> characterUnits;
        [SerializeField, ReadOnly] private ENextActionMode nextActionMode;
        [SerializeField, ReadOnly] private bool isPlaying;

        [Header("Pivots")]
        [SerializeField] private Transform left;
        [SerializeField] private Transform right;
        [SerializeField] private Transform up;
        [SerializeField] private Transform down;
        
        protected override void Internal_FromJson(Data data)
        {
            for (var i = 0; i < characterUnits.Count; i++)
            {
                var list = data.unitDataList;
                if(list.Count <= i) continue;
                if (string.IsNullOrEmpty(list[i].charKey) || string.IsNullOrEmpty(list[i].spriteKey)) 
                    continue;
                
                var sprite = ResourceProvider.Instance.GetSprite(list[i].spriteKey);
                DOTween.Sequence()
                    .SafeJoin(characterUnits[i].DOSwap(list[i].charKey, list[i].spriteKey, sprite, null, false))
                    .SafeJoin(characterUnits[i].DOMove(new Vector2(list[i].x, list[i].y), null, false))
                    .SafeJoin(characterUnits[i].DOScale(list[i].scl, null, false))
                    .SafeJoin(characterUnits[i].DOColor(list[i].color, list[i].fade, null, false));
            }
        }

        protected override ENextActionMode DefaultNextActionMode => ENextActionMode.Auto;

        protected override Data Internal_GetData() => new()
        {
            unitDataList = characterUnits.Select(x => x.CurrData).ToList()
        };

        public override void Initialize()
        {
            foreach (var unit in characterUnits) 
                unit.Initialize();

            nextActionMode = DefaultNextActionMode;
        }
        
        private (string, string) ParseKey(string rawKey)
        {
            var parsed = rawKey.Split(".");
            var charKey = parsed[0];
            var emotionKey = parsed.Length == 2 ? parsed[1] : string.Empty;
            var character = config.GetCharacter(charKey);
            if(character == null) charKey = null;
            var spriteResourceKey = character?.resourceKeyDict?.GetValueOrDefault(emotionKey);
            
            return (charKey, spriteResourceKey);
        }
        
        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            // key
            var (charKey, spriteResourceKey) = ParseKey(action.value);
            if (string.IsNullOrEmpty(charKey) || string.IsNullOrEmpty(spriteResourceKey))
            {
                Deb.LogError($"잘못된 캐릭터 Key입니다: {action.value}");
                return;
            }
            
            var sprite = ResourceProvider.Instance?.GetSprite(spriteResourceKey);
            
            // core
            var show = action.arguments.GetBool("show", true);
            nextActionMode = ParseNextActionMode(action);
            // anim
            var anim = action.arguments.GetBool("anim", true);
            var trans = action.arguments.GetString("trans", "fade");
            // x,y, scl
            var pos = new Vector2
            {
                x = action.arguments.GetFloat("x", 0.5f),
                y = action.arguments.GetFloat("y", 0.5f)
            };
            var scl = action.arguments.GetFloat("scl", 1f);
            // fade, color
            var fade = action.arguments.GetBool("fade", false);
            var color = action.arguments.GetColor("color", Color.white);
            color.a = 1f;
            
            var isNew = false;
            // 기존 유닛 찾기
            var unit = characterUnits.FirstOrDefault(u => u.CurrData.charKey == charKey);
            // 유닛이 없으면, 신규 가져오기(비활 중)
            if (!unit)
            {
                // 없는 상태에서 숨기려고 해도, 종료
                if (show == false) return;
                
                unit = characterUnits.FirstOrDefault(u => !u.gameObject.activeSelf);
                isNew = true;
            }
            // 그래도 없으면, 에러
            if (!unit)
            {
                Deb.LogError($"캐릭터 액션 실패: {charKey} / 최대 노출 가능한 캐릭터 수를 초과했습니다.");
                return;
            }
            
            // Emoji
            var emoji = action.arguments.GetString("emoji", "");
            UnityEngine.Sprite emojiSprite = null;
            if (emoji.Contains("img_emoji")) emojiSprite = ResourceProvider.Instance?.GetSprite(emoji);
            if (emojiSprite != null)
                unit.DOEmoji(emojiSprite, anim);

            // 재생
            isPlaying = true;
            seq = DOTween.Sequence(); 
            seq.SafeAppend(unit.DOSwap(charKey, spriteResourceKey, show ? sprite : null, null, anim));
            if (show)
            {
                var playAnim = anim && !isNew;
                seq.SafeJoin(unit.DOMove(GetPos(pos), null,playAnim ));
                seq.SafeJoin(unit.DOScale(scl, null, playAnim));
                seq.SafeJoin(unit.DOColor(color, fade, null, playAnim));
            }
            if (nextActionMode == ENextActionMode.Skip)
            {
                isPlaying = false;
                playNext?.Invoke(null);
            }
            else
            {
                seq.onComplete += () =>
                {
                    isPlaying = false;
                    if (nextActionMode == ENextActionMode.Auto)
                        playNext?.Invoke(null);
                };
            }

            return;
        }

        private Sequence seq;

        public override void SkipAction(bool isFastForward = false)
        {
            seq?.Complete(false);
            seq = null;
            
            foreach (var unit in characterUnits)
                if(unit.gameObject.activeSelf)
                    unit.Skip();
            isPlaying = false;
        }

        public override void ParseResources(ActionLine action, ref HashSet<string> sprites, ref HashSet<string> audios)
        {
            var (_, spriteResourceKey) = ParseKey(action.value);
            if (string.IsNullOrEmpty(spriteResourceKey))
            {
                Deb.LogWarning($"Parse Resource failed({action.value}). Key is empty.");
                return;
            }
            // Sprite
            sprites.Add(spriteResourceKey);
            
            // Emoji
            var emoji = action.arguments.GetString("emoji", "");
            if (!emoji.Contains("img_emoji")) emoji = "";
            if(!string.IsNullOrEmpty(emoji)) sprites.Add(emoji);
        }
        
        private Vector2 GetPos(Vector2 normal)
        {
            var x = left.localPosition.x + (right.localPosition.x - left.localPosition.x) * normal.x;
            var y = down.localPosition.y + (up.localPosition.y - down.localPosition.y) * normal.y;
            return new Vector2(x, y);
        }
    }
}