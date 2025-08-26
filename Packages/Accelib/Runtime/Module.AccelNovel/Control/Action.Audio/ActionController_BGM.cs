using System.Collections.Generic;
using Accelib.Logging;
using Accelib.Module.AccelNovel.Control.Resource;
using Accelib.Module.AccelNovel.Control.Utility;
using Accelib.Module.AccelNovel.Implements.SaveLoad.Collective;
using Accelib.Module.AccelNovel.Model;
using Accelib.Module.Audio;
using Accelib.Module.Audio.Data;
using Accelib.Module.SaveLoad;
using Accelix.Accelib.AccelNovel.Model;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.AccelNovel.Control.Action.Audio
{
    public sealed class ActionController_BGM : ActionControllerT<ActionController_BGM.Data>
    {
        [System.Serializable]
        public class Data
        {
            public string bgmKey;
            public float volume;
            public string effect;
        }

        [System.Serializable]
        public class Effect
        {
            public bool effEnabled = false;
            public float cutoffFrequency = 5000f;
            public float lowpassResonanceQ = 1f;
        }
        
        public override string Keyword => "bgm";
        public override bool IsPlaying => false;
        public override bool CanSkip => false;

        [Header("")]
        [SerializeField] private AudioRefSO audioRef;
        [SerializeField] private Data currData;
        
        [Header("BgmEffect")]
        [SerializeField] private SerializedDictionary<string, Effect> effDict;
        // [SerializeField, ReadOnly] private AudioLowPassFilter bgmLowPassFilter;

        // private void Awake() => bgmLowPassFilter = AudioSingleton.GetPlayerUnit(AudioChannel.Bgm)?.DefaultUnit?.LowPassFilter;

        protected override void Internal_FromJson(Data data)
        {
            currData = data;
            
            var audioClip = ResourceProvider.Instance.GetClip(currData.bgmKey);
            audioRef.SetClip(audioClip);
            audioRef.SwitchFade(false);
            
            Internal_SetLowPassFilter(currData.effect);
            AudioSingleton.SetControlVolume(audioRef.Channel, currData.volume);
        }

        protected override Data Internal_GetData() => currData;

        public override void Initialize()
        {
            currData = new Data();
            audioRef.SetClip(null);
        }

        public override void PlayAction(ActionLine action, UnityAction<string> playNext)
        {
            currData.bgmKey = action.value;
            var show = action.arguments.GetBool("show", true);
            var anim = action.arguments.GetBool("anim", true);
            currData.effect = action.arguments.GetString("effect", "none");
            currData.volume = Mathf.Clamp01(action.arguments.GetFloat("volume", 1f));

            if (!show)
            {
                currData.bgmKey = string.Empty;
                audioRef.Stop(anim);
                audioRef.SetClip(null);
                
                Internal_SetLowPassFilter(null);
            }
            else
            {
                var audioClip = ResourceProvider.Instance.GetClip(currData.bgmKey);
                if (audioClip == null)
                {
                    Deb.LogWarning($"오디오클립이 없습니다: {currData.bgmKey}", this);
                }
                else
                {
                    if (audioRef.Clip != audioClip)
                    {
                        audioRef.SetClip(audioClip);

                        if (anim)
                            audioRef.SwitchFade(false);
                        else
                            audioRef.Play(true);   
                    }
                    
                    Internal_SetLowPassFilter(currData.effect);
                    AudioSingleton.SetControlVolume(audioRef.Channel, currData.volume);
                    
                    // Collective
                    SaveLoadSingleton.Get<SaveDataHolder_Collective>().TryUnlock(currData.bgmKey, false, null);
                }
            }

            playNext?.Invoke(null);
        }

        public override void SkipAction(bool isFastForward = false) { }
        
        public override void Release()
        {
            // audioRef.SetClip(null);
            // audioRef.Stop(true);
            AudioSingleton.StopChannel(AudioChannel.Bgm, true);

            Internal_SetLowPassFilter(null);
        }

        public override void ParseResources(ActionLine action, ref HashSet<string> sprites, ref HashSet<string> audios)
        {
            if (!string.IsNullOrEmpty(action.value))
                audios.Add(action.value);
        }

        private void Internal_SetLowPassFilter(string effKey)
        {
            var eff = string.IsNullOrEmpty(effKey) ? null : effDict.GetValueOrDefault(effKey, null);
            eff ??= new Effect() { effEnabled = false, cutoffFrequency = 5000f, lowpassResonanceQ = 1f };

            // bgmLowPassFilter.enabled = eff.effEnabled;
            // bgmLowPassFilter.cutoffFrequency = eff.cutoffFrequency;
            // bgmLowPassFilter.lowpassResonanceQ = eff.lowpassResonanceQ;
        }
    }
}