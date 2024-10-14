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
        private const float MinDuplicateTime = 0.1f;
        
        [Header("Source")]
        [SerializeField, Range(1, 16)] private int maximumSourceCount = 1;
        [SerializeField, Range(0, 256)] private int priority = 128;
        [SerializeField] private EasePairTweenConfig fadeTweenConfig;
        [SerializeField] private List<AudioSource> sources;

        [Header("Volume")]
        [SerializeField, Range(0f, 1f)] private float fadeVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float defaultClipVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;

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
                
                var source = CreateSource(go.transform, unit.priority);
                unit.sources ??= new List<AudioSource>();
                unit.sources.Add(source);

                return unit;
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                return null;
            }
        }
        
        internal void PlayOneShot(AudioRefBase audioRef)
        {
            if(!audioRef?.Clip) return;

            var source = sources.FirstOrDefault(s => !s.isPlaying); // = sources.FirstOrDefault(x => x.isPlaying == false);
            if (!source)
            {
                if (sources.Count >= maximumSourceCount)
                {
                    Deb.LogWarning($"동시 재생 가능한 최대 사운드를 초과하여, 사운드를 재생하지 않습니다. [Name: {audioRef.Clip.name} / Channel: {audioRef.Channel} / Max: {sources.Count}]", this);
                    return;
                }

                source = CreateSource(transform, priority);
                sources.Add(source);
            }

            source.loop = false;
            source.resource = audioRef.Clip;
            UpdateVolume(source, audioRef.Volume);
            source.Play();
            
            // source.PlayOneShot(audioRef.Clip, audioRef.Volume);
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