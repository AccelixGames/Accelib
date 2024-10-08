using Accelib.Core;
using Accelib.Data;
using Accelib.Logging;
using Accelib.Module.Audio.Component;
using Accelib.Module.Audio.Data;
using Accelib.Module.Audio.Data._Base;
using AYellowpaper.SerializedCollections;
using UnityEngine;
// ReSharper disable Unity.PerformanceCriticalCodeInvocation

namespace Accelib.Module.Audio
{
    public class AudioSingleton : MonoSingleton<AudioSingleton>
    {
        [SerializeField] private EasePairTweenConfig fadeTweenConfig;
        
        [SerializeField, SerializedDictionary("채널", "유닛")]
        private SerializedDictionary<AudioChannel, AudioPlayerUnit> players;

        internal static void Play(in IAudioRef audioRef, bool fade)
        {
            if (!audioRef?.Clip) return;
            if (!TryGetInstance(out var instance)) return;

            instance.GetPlayer(in audioRef)?.Play(audioRef, fade);
        }

        internal static void PlayOneShot(in IAudioRef audioRef)
        {
            if (!audioRef?.Clip) return;
            if (!TryGetInstance(out var instance)) return;

            instance.GetPlayer(in audioRef)?.PlayOneShot(audioRef);
        }

        internal static void Stop(in IAudioRef audioRef, bool fade)
        {
            if (!TryGetInstance(out var instance)) return;
            
            instance.GetPlayer(in audioRef)?.Stop(fade);
        }

        internal static void SwitchFade(in IAudioRef audioRef)
        {
            if (audioRef?.Clip == null) return;
            if (!TryGetInstance(out var instance)) return;
            
            instance.GetPlayer(in audioRef)?.SwitchFade(audioRef);
        }

        private AudioPlayerUnit GetPlayer(in IAudioRef audioRef)
        {
            // 가져오기 실패했다면,
            if (!players.TryGetValue(audioRef.Channel, out var player))
            {
                // 생성
                player = AudioPlayerUnit.CreateInstance(transform, $"(AudioPlayerUnit) {audioRef.Channel}", fadeTweenConfig);

                // 에러 체크
                if (player == null)
                {
                    Deb.LogError("AudioPlayerUnit 생성에 실패했습니다.", this);
                    return null;
                }

                players.TryAdd(audioRef.Channel, player);
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