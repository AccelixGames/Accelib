using System.Collections.Generic;
using Accelib.Logging;
using Accelib.Module.AccelNovel.Control.Resource;
using Accelib.Module.AccelNovel.Control.Utility;
using Accelib.Module.AccelNovel.Model;
using Accelib.Module.Audio;
using Accelib.Module.Audio.Data;
using Accelix.Accelib.AccelNovel.Model;
using Accelix.Accelib.AccelNovel.Model.Enum;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Audio
{
    public sealed class ActionController_SFX : ActionController_Default
    {
        public override string Keyword => "sfx";
        public override bool IsPlaying => isPlaying;
        protected override ENextActionMode DefaultNextActionMode => ENextActionMode.Skip;
        public override bool CanSkip => nextMode == ENextActionMode.Auto && audioRef.Clip != null;

        [SerializeField] private AudioRefSO audioRef;
        [SerializeField, ReadOnly] private bool isPlaying;
        [SerializeField, ReadOnly] private ENextActionMode nextMode;
        
        private Sequence _seq;
        
        public override void Initialize()
        {
            audioRef.SetClip(null);
            _seq?.Kill();
        }

        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            var key = action.value;
            var delay = action.arguments.GetFloat("delay", 0f);
            var volume = Mathf.Clamp01(action.arguments.GetFloat("volume", 1f));
            nextMode = ParseNextActionMode(action);
            
            var audioClip = ResourceProvider.Instance.GetClip(key);
            if (audioClip == null)
            {
                Deb.LogError($"AudioClip {key} is not found.", this);
                playNext?.Invoke(null);
                return;
            }

            _seq = DOTween.Sequence().SetLink(gameObject)
                .AppendInterval(delay)
                .AppendCallback(() =>
                {
                    AudioSingleton.SetControlVolume(audioRef.Channel, volume);   
                    audioRef.SetClip(audioClip);
                    audioRef.Play();
                    //Deb.Log($"SFX 재생: {audioClip.name}(딜레이:{delay}/음량:{volume})", this);
                });

            if (nextMode == ENextActionMode.Auto)
            {
                _seq.AppendInterval(audioClip.length);
                _seq.AppendCallback(() => playNext?.Invoke(null));
            }
            else
            {
                playNext?.Invoke(null);
                _seq = null;
            }
        }

        public override void SkipAction(bool isFastForward = false)
        {
            if(isFastForward && audioRef.Clip != null)
                audioRef.Stop(true);
            
            _seq?.Kill(true);
            isPlaying = false;
        }
        
        public override void Release()
        {
            SkipAction();
            // audioRef.SetClip(null);
            // audioRef.Stop(true);
            AudioSingleton.StopChannel(AudioChannel.Sfx_Effect, true);
        }
        
        public override void ParseResources(ActionLine action, ref HashSet<string> sprites, ref HashSet<string> audios)
        {
            if (!string.IsNullOrEmpty(action.value))
                audios.Add(action.value);
        }
    }
}