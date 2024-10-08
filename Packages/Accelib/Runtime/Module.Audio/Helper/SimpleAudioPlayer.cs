using Accelib.Module.Audio.Data;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.Audio.Helper
{
    public class SimpleAudioPlayer : MonoBehaviour
    {
        private enum Mode
        {
            None,
            Play,
            PlayFadeIn,
            SwitchFade,
            PlayOneShot
        }

        [SerializeField] private Mode onEnableMode = Mode.None;
        [SerializeField] private AudioRefSO audioRef;

        [Button(enabledMode: EButtonEnableMode.Playmode)] private void Play() => audioRef?.Play();
        [Button(enabledMode: EButtonEnableMode.Playmode)] private void PlayFadeIn() => audioRef?.Play(true);
        [Button(enabledMode: EButtonEnableMode.Playmode)] private void Stop() => audioRef?.Stop();
        [Button(enabledMode: EButtonEnableMode.Playmode)] private void StopFadeOut() => audioRef?.Stop(true);
        [Button(enabledMode: EButtonEnableMode.Playmode)] private void SwitchFade() => audioRef?.SwitchFade();
        [Button(enabledMode: EButtonEnableMode.Playmode)] private void PlayOneShot() => audioRef?.PlayOneShot();

        private void OnEnable()
        {
            if (onEnableMode == Mode.Play) Play();
            else if(onEnableMode == Mode.PlayFadeIn) PlayFadeIn();
            else if(onEnableMode == Mode.SwitchFade) SwitchFade();
            else if (onEnableMode == Mode.PlayOneShot) PlayOneShot();
        }

        private void Reset()
        {
            // gameObject.name = "(SimpleAudioPlayer) Default";
        }
    }
}