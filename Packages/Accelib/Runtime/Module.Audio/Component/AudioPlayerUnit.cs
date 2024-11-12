using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Data;
using Accelib.Logging;
using Accelib.Module.Audio.Data._Base;
using DG.Tweening;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable InconsistentNaming

namespace Accelib.Module.Audio.Component
{
    internal class AudioPlayerUnit : MonoBehaviour
    {
        [Header("Option")]
        [SerializeField] private EasePairTweenConfig fadeTweenConfig;
        [SerializeField, Range(1, 16)] private int maximumSourceCount = 1;
        [SerializeField, Range(0, 256)] private int priority = 128;
        [SerializeField, Range(0.01f, 1f)] private float minDuplicateTime = 0.1f;

        #region Volume
        [Header("Volume")]
        [SerializeField, ReadOnly, Range(0f, 1f)] private float _totalVolume = 1f;
        [SerializeField, Range(0f, 1f), ReadOnly] private float _fadeVolume = 1f;
        [SerializeField, Range(0f, 1f), ReadOnly] private float _controlVolume = 1f;
        [SerializeField] private FloatVariable masterVolume;
        private float FadeVolume
        {
            get => _fadeVolume;
            set { _fadeVolume = Mathf.Clamp01(value); UpdateVolumes(); }
        }
        private float ControlVolume
        {
            get => _controlVolume;
            set { _controlVolume = Mathf.Clamp01(value); UpdateVolumes(); }
        }
        private void UpdateVolumes()
        {
             var newVolume = FadeVolume * masterVolume.Value * ControlVolume;
             if(Mathf.Abs(newVolume - _totalVolume) < 0.01f) return;
             
             _totalVolume = newVolume; 
            foreach (var source in units) 
                source.UpdateVolume(_totalVolume);
        }
        
        public Tweener SetControlVolume(float value)
        {
            return DOTween
                .To(() => ControlVolume, x => ControlVolume = x, value, fadeTweenConfig.duration)
                .SetEase(fadeTweenConfig.easeA)
                .OnComplete(() => ControlVolume = value)
                .SetLink(gameObject);
        }
        #endregion

        [Header("Sources")]
        [SerializeField, ReadOnly] private List<AudioSourceUnit> units;
        private AudioSourceUnit DefaultUnit => units[0];

        private Sequence _fadeSeq;
        
        internal static AudioPlayerUnit CreateInstance(Transform parent, string name, EasePairTweenConfig fadeTweenConfig)
        {
            try
            {
                var go = new GameObject(name,  typeof(AudioPlayerUnit));
                go.transform.SetParent(parent);
                
                var unit = go.GetComponent<AudioPlayerUnit>();
                
                unit.FadeVolume = 1f;
                unit.ControlVolume = 1f;
                
                unit.fadeTweenConfig = fadeTweenConfig;
                unit.units = new List<AudioSourceUnit>();

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
            units = new List<AudioSourceUnit>();
            for (var i = 0; i < maximumSourceCount; i++) 
                units.Add(AudioSourceUnit.Create(transform, priority));
        }

        private void OnEnable() => masterVolume.Changed.Register(UpdateVolumes);
        private void OnDisable() => masterVolume.Changed.Register(UpdateVolumes);

        internal void PlayOneShot(AudioRefBase audioRef, float delay = 0f)
        {
            if (audioRef.Clip == null)
            {
                if(audioRef.ShowLog)
                    Deb.LogWarning($"클립이 없어 재생하지 않습니다. [Name: {audioRef.Clip.name} / Channel: {audioRef.Channel} / Max: {units.Count}]", this);
                return;
            }

            // 현재 동일한 소리가 플레이중이라면, 스킵
            foreach (var s in units.Where(x=>x.Source.isPlaying && x.Source.clip == audioRef.Clip))
                if (s.Source.time <= minDuplicateTime)
                {
                    if(audioRef.ShowLog)
                        Deb.LogWarning($"현재 동일한 소리가 재생중입니다. 재생하지 않습니다. 재생중인 소스({s.Source}, {s.Source.clip}) [Name: {audioRef.Clip.name} / Channel: {audioRef.Channel} / Max: {units.Count}]", s);
                    return;
                }
            
            // 소스를 찾지 못했다면, 스킵
            var currFrame = Time.frameCount;
            foreach (var unit in units)
            {
                if(unit.Source.isPlaying) continue;
                if(unit.LastPlayFrame >= currFrame) continue;
                
                unit.Play(audioRef, _totalVolume, false, delay);
                return;
            }
            
            if(audioRef.ShowLog)
                Deb.LogWarning($"동시 재생 가능한 최대 사운드를 초과하여, 사운드를 재생하지 않습니다. [Name: {audioRef.Clip.name} / Channel: {audioRef.Channel} / Max: {units.Count}]", this);
        }

        internal void Play(AudioRefBase audioRef, bool fade)
        {
            _fadeSeq?.Kill();

            if (fade)
            {
                PlayFadeIn(audioRef);
            }
            else
            {
                FadeVolume = 1f;
                DefaultUnit.Play(audioRef, _totalVolume);
            }
        }
        
        internal void Stop(bool fade)
        {
            _fadeSeq?.Kill();

            if (fade)
            {
                StopFadeOut();
            }
            else
            {
                FadeVolume = 1f;
                DefaultUnit.Stop(_totalVolume);
            }
        }

        private Tweener PlayFadeIn(AudioRefBase audioRef) => DOTween
            .To(() => FadeVolume, x => FadeVolume = x, 1f, fadeTweenConfig.duration)
            .SetEase(fadeTweenConfig.easeA)
            .OnStart(() => DefaultUnit.Play(audioRef, _totalVolume))
            .SetLink(gameObject);

        private Tweener StopFadeOut() => DOTween
            .To(() => FadeVolume, x => FadeVolume = x, 0f, fadeTweenConfig.duration)
            .SetEase(fadeTweenConfig.easeB)
            .OnComplete(() => DefaultUnit.Stop(_totalVolume))
            .SetLink(gameObject);
        
        internal void SwitchFade(AudioRefBase audioRef, bool skipOnSame)
        {
            _fadeSeq?.Kill();
            _fadeSeq = DOTween.Sequence();

            // 현재 재생중이라면,
            if (DefaultUnit.Source.isPlaying)
            {
                // 동일한 것 재생시 스킵
                if (skipOnSame && audioRef.Clip == DefaultUnit.Source.clip)
                    return;
                
                // 볼륨 줄이며 멈추는 트윈 추가
                _fadeSeq.Append(StopFadeOut());
            }
            else
            {
                // 바로 볼륨 0으로 만들기
                _fadeSeq.AppendCallback(() =>
                {
                    FadeVolume = 0f;
                    DefaultUnit.Stop(_totalVolume);
                });
            }
            
            // 볼륨 키우며 재생하는 트윈 추가
            _fadeSeq.Append(PlayFadeIn(audioRef));
        }
    }
}