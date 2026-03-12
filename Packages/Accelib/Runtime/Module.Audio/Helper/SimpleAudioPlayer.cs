using Accelib.Module.Audio.Data;
using Sirenix.OdinInspector;
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

        [Button, EnableIf("@UnityEngine.Application.isPlaying")] private void Play() => audioRef?.Play();
        [Button, EnableIf("@UnityEngine.Application.isPlaying")] private void PlayFadeIn() => audioRef?.Play(true);
        [Button, EnableIf("@UnityEngine.Application.isPlaying")] private void Stop() => audioRef?.Stop();
        [Button, EnableIf("@UnityEngine.Application.isPlaying")] private void StopFadeOut() => audioRef?.Stop(true);
        [Button, EnableIf("@UnityEngine.Application.isPlaying")] private void SwitchFade() => audioRef?.SwitchFade();
        [Button, EnableIf("@UnityEngine.Application.isPlaying")] private void PlayOneShot() => audioRef?.PlayOneShot();

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