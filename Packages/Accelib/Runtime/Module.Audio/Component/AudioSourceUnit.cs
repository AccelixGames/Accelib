using Accelib.Logging;
using Accelib.Module.Audio.Data;
using Accelib.Module.Audio.Data._Base;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.Audio.Component
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceUnit : MonoBehaviour
    {
        [field: SerializeField] public AudioSource Source { get; private set; }

        [field: Header("Refs")]
        [field: SerializeField, ReadOnly] public AudioRefBase CurrRef { get; private set; }
        [field: SerializeField, ReadOnly] public int LastPlayFrame { get; private set; } = 0;
        
        public static AudioSourceUnit Create(Transform parent, int priority)
        {
            var unit = new GameObject("", typeof(AudioSource), typeof(AudioSourceUnit)).GetComponent<AudioSourceUnit>();
            unit.Source = unit.GetComponent<AudioSource>();
            
            unit.Source.transform.SetParent(parent);
            unit.Source.transform.SetAsLastSibling();
            unit.Source.name = $"AudioSourceUnit({unit.Source.transform.GetSiblingIndex()})";
            unit.Source.mute = false;
            unit.Source.priority = priority;
            unit.Source.playOnAwake = false;
            unit.Source.reverbZoneMix = 0f;
            unit.Source.dopplerLevel = 0f;
            unit.Source.rolloffMode = AudioRolloffMode.Linear;
            unit.Source.minDistance = 0f;
            unit.Source.maxDistance = 1f;
            unit.Source.volume = 1f;

            return unit;
        }

        public void Play(AudioRefBase audioRef, float volume, bool loop = true, float delay = 0)
        {
            LastPlayFrame = Time.frameCount;
            CurrRef = audioRef;
            Source.loop = loop && audioRef.Loop;
            Source.clip = audioRef.Clip;
            
            UpdateVolume(volume);
            if(delay > 0)
                Source.PlayDelayed(delay);
            else
                Source.Play();
            
            #if UNITY_EDITOR
            if(audioRef.ShowLog)
                Deb.Log($"Play {audioRef.name} on {name}", this);
            #endif
        }

        public void Stop(float volume)
        {
            CurrRef = null;
            Source.clip = null;
            
            Source.Stop();
            UpdateVolume(volume);
        }

        public void UpdateVolume(float volume) => Source.volume = volume * (CurrRef?.Volume ?? 1f);
    }
}