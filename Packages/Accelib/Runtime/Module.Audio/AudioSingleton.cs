﻿using Accelib.Core;
using Accelib.Data;
using Accelib.Logging;
using Accelib.Module.Audio.Component;
using Accelib.Module.Audio.Data;
using Accelib.Module.Audio.Data._Base;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
// ReSharper disable Unity.PerformanceCriticalCodeInvocation

namespace Accelib.Module.Audio
{
    public class AudioSingleton : MonoSingleton<AudioSingleton>
    {
        [SerializeField] private EasePairTweenConfig fadeTweenConfig;
        
        [SerializeField, SerializedDictionary("채널", "유닛")]
        private SerializedDictionary<AudioChannel, AudioPlayerUnit> players;

        internal static void Play(in AudioRefBase audioRef, bool fade)
        {
            if (!audioRef?.Clip) return;
            if (!TryGetInstance(out var instance)) return;

            instance.GetPlayer(in audioRef)?.Play(audioRef, fade);
        }

        internal static void PlayOneShot(in AudioRefBase audioRef, float delay = 0f)
        {
            if (!TryGetInstance(out var instance))
            {
                Deb.LogError("AudioSingleton Instance is null", audioRef);
                return;
            }

            instance?.GetPlayer(in audioRef)?.PlayOneShot(audioRef, delay);
        }

        internal static void Stop(in AudioRefBase audioRef, bool fade)
        {
            if (!TryGetInstance(out var instance)) return;
            
            instance.GetPlayer(in audioRef)?.Stop(fade);
        }

        public static Tweener SetControlVolume(AudioChannel channel, float volume) =>
            Instance.GetPlayer(channel).SetControlVolume(volume);

        public static void StopChannel(AudioChannel channel, bool fade)
        {
            if (!TryGetInstance(out var instance)) return;

            instance.GetPlayer(channel)?.Stop(fade);
        }

        internal static void SwitchFade(in AudioRefBase audioRef, bool skipOnSame)
        {
            if (audioRef?.Clip == null) return;
            if (!TryGetInstance(out var instance)) return;
            
            instance.GetPlayer(in audioRef)?.SwitchFade(audioRef, skipOnSame);
        }

        private AudioPlayerUnit GetPlayer(in AudioRefBase audioRef) => 
            audioRef ? GetPlayer(audioRef.Channel) : null;

        private AudioPlayerUnit GetPlayer(AudioChannel channel)
        {
            // 가져오기 실패했다면,
            if (!players.TryGetValue(channel, out var player))
            {
                // 생성
                player = AudioPlayerUnit.CreateInstance(transform, $"(AudioPlayerUnit) {channel}", fadeTweenConfig);

                // 에러 체크
                if (player == null)
                {
                    Deb.LogError("AudioPlayerUnit 생성에 실패했습니다.", this);
                    return null;
                }

                players.TryAdd(channel, player);
            }

            // 반환
            return player;
        }

#if UNITY_EDITOR
        private void Reset() => gameObject.name = "(Singleton) Audio";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => Initialize();
#endif
    }
}