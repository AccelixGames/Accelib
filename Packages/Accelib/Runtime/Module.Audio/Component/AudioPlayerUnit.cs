using System;
using Accelib.Data;
using Accelib.Logging;
using Accelib.Module.Audio.Data._Base;
using DG.Tweening;
using UnityEngine;

namespace Accelib.Module.Audio.Component
{
    [RequireComponent(typeof(AudioSource))]
    internal class AudioPlayerUnit : MonoBehaviour
    {
        [Header("Source")]
        [SerializeField] private AudioSource source;
        [SerializeField] private EasePairTweenConfig fadeTweenConfig;

        [Header("Volume")]
        [SerializeField, Range(0f, 1f)] private float fadeVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float clipVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;

        private Sequence _seq;
        
        internal static AudioPlayerUnit CreateInstance(Transform parent, string name, EasePairTweenConfig fadeTweenConfig)
        {
            try
            {
                var go = new GameObject(name, typeof(AudioSource), typeof(AudioPlayerUnit));
                go.transform.SetParent(parent);

                var source = go.GetComponent<AudioSource>();
                source.mute = false;
                source.playOnAwake = false;
                source.reverbZoneMix = 0f;
                source.dopplerLevel = 0f;
                source.rolloffMode = AudioRolloffMode.Linear;
                source.minDistance = 0f;
                source.maxDistance = 1f;
                source.volume = 1f;
                
                var unit = go.GetComponent<AudioPlayerUnit>();
                unit.source = source;
                unit.fadeVolume = 1f;
                unit.clipVolume = 1f;
                unit.masterVolume = 1f;
                unit.fadeTweenConfig = fadeTweenConfig;

                return unit;
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                return null;
            }
        }
        
        internal void PlayOneShot(IAudioRef audioRef) => source.PlayOneShot(audioRef.Clip, audioRef.Volume);

        internal void Play(IAudioRef audioRef, bool fade)
        {
            _seq?.Kill();

            if (fade)
            {
                PlayFadeIn(audioRef);
            }
            else
            {
                source.clip = audioRef.Clip;
                source.loop = audioRef.Loop;
                
                fadeVolume = 1f;
                clipVolume = audioRef.Volume;
                UpdateVolume();
                
                source.Play();
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
                source.Stop();
                source.clip = null;
            
                fadeVolume = 1f;
                clipVolume = 1f;
                UpdateVolume();
            }
        }

        internal void SwitchFade(IAudioRef audioRef, bool skipOnSame)
        {
            _seq?.Kill();
            _seq = DOTween.Sequence();

            // 현재 재생중이라면,
            if (source.isPlaying)
            {
                // 동일한 것 재생시 스킵
                if (skipOnSame && audioRef.Clip == source.clip)
                    return;
                
                // 볼륨 줄이며 멈추는 트윈 추가
                _seq.Append(StopFadeOut());
            }
            else
            {
                // 바로 볼륨 0으로 만들기
                _seq.AppendCallback(() =>
                {
                    source.Stop();
                    source.clip = null;
            
                    fadeVolume = 0f;
                    clipVolume = 1f;
                    UpdateVolume();
                });
            }
            
            // 볼륨 키우며 재생하는 트윈 추가
            _seq.Append(PlayFadeIn(audioRef));
        }

        private Tweener PlayFadeIn(IAudioRef audioRef) => DOTween
            .To(() => fadeVolume, x => fadeVolume = x, 1f, fadeTweenConfig.duration)
            .SetEase(fadeTweenConfig.easeA)
            .OnUpdate(UpdateVolume)
            .OnStart(() =>
            {
                source.clip = audioRef.Clip;
                source.loop = audioRef.Loop;
                clipVolume = audioRef.Volume;
                source.Play();
            })
            .SetLink(gameObject);

        private Tweener StopFadeOut() => DOTween
            .To(() => fadeVolume, x => fadeVolume = x, 0f, fadeTweenConfig.duration)
            .SetEase(fadeTweenConfig.easeB)
            .OnUpdate(UpdateVolume)
            .OnComplete(source.Stop)
            .SetLink(gameObject);
        
        private void UpdateVolume() => source.volume = Mathf.Clamp01(fadeVolume * clipVolume * masterVolume);
    }
}