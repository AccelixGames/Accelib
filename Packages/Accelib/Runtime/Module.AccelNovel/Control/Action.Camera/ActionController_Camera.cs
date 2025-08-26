using System.Collections.Generic;
using Accelib.Data;
using Accelib.Module.AccelNovel.Control.Utility;
using Accelib.Module.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model.Enum;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Camera
{
    public class ActionController_Camera : ActionController_Default
    {
        public override string Keyword => "cam";
        public override bool IsPlaying => isPlaying;

        public override bool CanSkip => true;
        protected override ENextActionMode DefaultNextActionMode => ENextActionMode.Skip;

        [Header("Action")]
        [SerializeField] private CamController camController;
        [SerializeField] private ShakeTweenConfig defaultEff;
        [SerializeField] private SerializedDictionary<string, ShakeTweenConfig> effDict;

        [Header("Value")]
        [SerializeField] private bool isPlaying;

        private Tweener _camTween;

        public override void Initialize()
        {
            isPlaying = false;
        }

        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            var effName = action.value;
            var eff = effDict.GetValueOrDefault(effName, defaultEff);
            
            var duration = action.arguments.GetFloat("duration", eff.duration);
            var str = Vector3.zero;
            str.x = eff.strength.x * action.arguments.GetFloat("x", 1f);
            str.y = eff.strength.y * action.arguments.GetFloat("y", 1f);
            var vibrato = (int)(eff.vibrato * action.arguments.GetFloat("vibrato", 1f));
            
            var next = ParseNextActionMode(action);

            _camTween?.Kill();
            isPlaying = true;
            _camTween = camController.DoShake(duration, str, vibrato, eff.randomness, eff.fadeOut);

            if (next == ENextActionMode.Skip)
            {
                isPlaying = false;
                playNext?.Invoke(null);
            }
            else
            {
                _camTween.OnComplete(() =>
                {
                    isPlaying = false;
                    playNext?.Invoke(null);
                });   
            }
        }

        public override void SkipAction(bool isFastForward = false)
        {
            if(isFastForward)
                _camTween?.Complete(true);
        }
        
        #if UNITY_EDITOR
        [Button]
        private void Test_Shake()
        {
            var eff = defaultEff;

            var duration = eff.duration;
            var str = Vector3.zero;
            str.x = eff.strength.x * 1f;
            str.y = eff.strength.y * 1f;
            var vibrato = (int)(eff.vibrato * 1f);

            _camTween?.Kill();
            _camTween = camController.DoShake(duration, str, vibrato, eff.randomness, eff.fadeOut);
        }
        #endif
    }
}