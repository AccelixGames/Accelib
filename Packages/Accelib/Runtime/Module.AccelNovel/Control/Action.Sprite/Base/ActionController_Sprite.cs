using System.Collections.Generic;
using Accelib.Extensions;
using Accelib.Logging;
using Accelib.Module.AccelNovel.Control.Action.Sprite.Unit;
using Accelib.Module.AccelNovel.Control.Resource;
using Accelib.Module.AccelNovel.Control.Utility;
using Accelib.Module.AccelNovel.Implements.SaveLoad.Collective;
using Accelib.Module.AccelNovel.Model;
using Accelib.Module.SaveLoad;
using Accelix.Accelib.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model.Enum;
using DG.Tweening;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Sprite.Base
{
    public abstract class ActionController_Sprite : ActionControllerT<ActionController_Sprite.Data>
    {
        [System.Serializable]
        public class Data
        {
            public string spriteKey;
            [JsonConverter(typeof(ColorConverter))] public Color color;
        }
        
        public override bool IsPlaying => isPlaying;
        protected abstract bool IsUnlockCollective { get; }

        [SerializeField] private SpriteRenderUnit unit;
        [SerializeField, ReadOnly] private bool isPlaying;
        [SerializeField, ReadOnly] private Data currData;

        private Sequence _seq;
        
        protected override void Internal_FromJson(Data data)
        {
            currData = data;
            if (!string.IsNullOrEmpty(currData?.spriteKey))
            {
                var sprite = ResourceProvider.Instance?.GetSprite(currData?.spriteKey);
                unit.DoSwap(sprite, null, false);
            }
        }

        protected override Data Internal_GetData() => currData;
        
        public override void Initialize()
        {
            unit.Initialize();
            isPlaying = false;
            currData = new Data();
        }

        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            // core
            var show = action.arguments.GetBool("show", true);
            var next = ParseNextActionMode(action);
            // anim
            var anim = action.arguments.GetBool("anim", true);
            var trans = action.arguments.GetString("trans", "fade");
            var colorStr = action.arguments.GetString("color", "#FFFFFF");
            var delay = action.arguments.GetFloat("delay", 0f);
            var afterDelay = action.arguments.GetFloat("afterDelay", 0f);
            
            if(!ColorUtility.TryParseHtmlString(colorStr, out var color))
                color = Color.white;
            color.a = 1f;

            // Sprite
            var sprite = !show || string.IsNullOrEmpty(action.value)
                ? null
                : ResourceProvider.Instance?.GetSprite(action.value);
            
            // Core
            currData.spriteKey = sprite != null ? action.value : null;
            currData.color = color;
            
            // Collective
            if (IsUnlockCollective && sprite != null)
            {
                SaveLoadSingleton.Get<SaveDataHolder_Collective>().TryUnlock(action.value, false, null);
                Novel.OnViewCG(action.value);
            }
            
            // 이미지 스왑 시퀀스 생성
            if (unit.IsSameSprite(sprite))
            {
                _seq = unit.DOColor(color, null, anim);
            }
            else
            {
                _seq = unit.DoSwap(sprite, null, anim);
                _seq.PrependCallback(() => unit.SetColor(color));    
            }
            
            // 딜레이
            if (delay > 0f)
                _seq.PrependInterval(delay);
            if(afterDelay > 0f)
                _seq.AppendInterval(afterDelay);
            
            // 스킵이 아닐 때, 재생으로 설정
            isPlaying = next != ENextActionMode.Skip;
            if (isPlaying)
            {
                // 시퀀스가 끝났을 때
                _seq.OnComplete(() =>
                {
                    // 재생 종료
                    isPlaying = false;
                    // 자동이라면, 다음 재생
                    if (next == ENextActionMode.Auto)
                        playNext?.Invoke(null);
                });
            }
            else
            {
                // 스킵하고 바로 다음 재생
                playNext?.Invoke(null);
            }
        }

        public override void SkipAction(bool isFastForward = false)
        {
            _seq?.Complete(true);
            isPlaying = false;
        }

        public override void ParseResources(ActionLine action, ref HashSet<string> sprites, ref HashSet<string> audios)
        {
            if (string.IsNullOrEmpty(action.value))
            {
                Deb.LogWarning($"Parse Resource failed({action.key}). Key is empty.");
                return;
            }
            
            sprites.Add(action.value);
        }
    }
}