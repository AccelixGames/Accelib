using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Data;
using Accelib.Logging;
using Accelib.Module.Audio.Data._Base;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.Audio.Component
{
    internal class AudioPlayerUnit : MonoBehaviour
    {
        [Header("Option")]
        [SerializeField] private EasePairTweenConfig fadeTweenConfig;
        [SerializeField, Range(1, 16)] private int maximumSourceCount = 1;
        [SerializeField, Range(0, 256)] private int priority = 128;
        [SerializeField, Range(0.01f, 1f)] private float minDuplicateTime = 0.1f;

        [Header("Volume")]
        [SerializeField, Range(0f, 1f), ReadOnly] private float fadeVolume = 1f;
        [SerializeField, Range(0f, 1f), ReadOnly] private float defaultClipVolume = 1f;
        [SerializeField, Range(0f, 1f), ReadOnly] private float masterVolume = 1f;
        
        [Header("Sources")]
        [SerializeField, ReadOnly] private List<AudioSource> sources;

        private Sequence _seq;

        private AudioSource DefaultSource => sources[0];

        private static AudioSource CreateSource(Transform parent, int priority)
        {
            var source = new GameObject("AudioSource", typeof(AudioSource)).GetComponent<AudioSource>();
            source.transform.SetParent(parent);
            source.transform.SetAsLastSibling();
            source.name = $"AudioSource({source.transform.GetSiblingIndex()})";
            source.mute = false;
            source.priority = priority;
            source.playOnAwake = false;
            source.reverbZoneMix = 0f;
            source.dopplerLevel = 0f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = 0f;
            source.maxDistance = 1f;
            source.volume = 1f;

            return source;
        }
        
        internal static AudioPlayerUnit CreateInstance(Transform parent, string name, EasePairTweenConfig fadeTweenConfig)
        {
            try
            {
                var go = new GameObject(name,  typeof(AudioPlayerUnit));
                go.transform.SetParent(parent);
                
                var unit = go.GetComponent<AudioPlayerUnit>();
                unit.fadeVolume = 1f;
                unit.defaultClipVolume = 1f;
                unit.masterVolume = 1f;
                unit.fadeTweenConfig = fadeTweenConfig;
                unit.sources = new List<AudioSource>();
                
                // var source = CreateSource(go.transform, unit.priority);
                //unit.sources.Add(source);

                return unit;
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                return null;
            }
        }

        private void Awake()
        {
            sources = new List<AudioSource>();
            for (var i = 0; i < maximumSourceCount; i++) 
                sources.Add(CreateSource(transform, priority));
        }

        internal void PlayOneShot(AudioRefBase audioRef)
        {
            if(!audioRef?.Clip) return;

            // 현재 동일한 소리가 플레이중이라면, 스킵
            foreach (var s in sources.Where(x=>x.isPlaying && x.clip == audioRef.Clip))
                if (s.time <= minDuplicateTime)
                    return;
            
            // 소스를 찾지 못했다면, 스킵
            var source = sources.FirstOrDefault(s => !s.isPlaying);
            if (!source)
            {
                Deb.LogWarning($"동시 재생 가능한 최대 사운드를 초과하여, 사운드를 재생하지 않습니다. [Name: {audioRef.Clip.name} / Channel: {audioRef.Channel} / Max: {sources.Count}]", this);
                return;
            }

            // 재생
            source.loop = false;
            source.resource = audioRef.Clip;
            UpdateVolume(source, audioRef.Volume);
            source.Play();
        }

        internal void Play(AudioRefBase audioRef, bool fade)
        {
            _seq?.Kill();

            if (fade)
            {
                PlayFadeIn(audioRef);
            }
            else
            {
                DefaultSource.resource = audioRef.Clip;
                DefaultSource.loop = audioRef.Loop;
                
                fadeVolume = 1f;
                defaultClipVolume = audioRef.Volume;
                UpdateVolume( DefaultSource, defaultClipVolume);
                
                DefaultSource.Play();
            }
        }
        
        internal void Stop(bool fade)
        {
            _seq?.Kill();

            if (fade)
            {
                StopFadeOut();
            }
            else
            {
                DefaultSource.Stop();
                DefaultSource.clip = null;
            
                fadeVolume = 1f;
                defaultClipVolume = 1f;
                UpdateVolume( DefaultSource, defaultClipVolume);
            }
        }

        internal void SwitchFade(AudioRefBase audioRef, bool skipOnSame)
        {
            _seq?.Kill();
            _seq = DOTween.Sequence();

            // 현재 재생중이라면,
            if (DefaultSource.isPlaying)
            {
                // 동일한 것 재생시 스킵
                if (skipOnSame && audioRef.Clip == DefaultSource.clip)
                    return;
                
                // 볼륨 줄이며 멈추는 트윈 추가
                _seq.Append(StopFadeOut());
            }
            else
            {
                // 바로 볼륨 0으로 만들기
                _seq.AppendCallback(() =>
                {
                    DefaultSource.Stop();
                    DefaultSource.clip = null;
            
                    fadeVolume = 0f;
                    defaultClipVolume = 1f;
                    UpdateVolume( DefaultSource, defaultClipVolume);
                });
            }
            
            // 볼륨 키우며 재생하는 트윈 추가
            _seq.Append(PlayFadeIn(audioRef));
        }

        private Tweener PlayFadeIn(AudioRefBase audioRef) => DOTween
            .To(() => fadeVolume, x => fadeVolume = x, 1f, fadeTweenConfig.duration)
            .SetEase(fadeTweenConfig.easeA)
            .OnUpdate(() => UpdateVolume( DefaultSource, defaultClipVolume))
            .OnStart(() =>
            {
                DefaultSource.resource = audioRef.Clip;
                DefaultSource.loop = audioRef.Loop;
                defaultClipVolume = audioRef.Volume;
                DefaultSource.Play();
            })
            .SetLink(gameObject);

        private Tweener StopFadeOut() => DOTween
            .To(() => fadeVolume, x => fadeVolume = x, 0f, fadeTweenConfig.duration)
            .SetEase(fadeTweenConfig.easeB)
            .OnUpdate(() => UpdateVolume( DefaultSource, defaultClipVolume))
            .OnComplete(DefaultSource.Stop)
            .SetLink(gameObject);
        
        private void UpdateVolume(AudioSource source, float clipVolume) => source.volume = Mathf.Clamp01(fadeVolume * clipVolume * masterVolume);

        [Button]
        private void DebSources()
        {
            foreach (var source in sources)
            {
                Deb.Log($"{source.name}: {source.clip}, {source.isPlaying}");
            }
        }
    }
}