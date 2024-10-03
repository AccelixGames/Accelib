﻿using System;
using Accelib.Audio.Data;
using Accelib.Logging;
using UnityEngine;

namespace Accelib.Audio.Component
{
    [RequireComponent(typeof(AudioSource))]
    internal class AudioPlayerUnit : MonoBehaviour
    {
        [SerializeField] private AudioSource source;

        internal static AudioPlayerUnit CreateInstance(Transform parent, string name)
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

                var unit = go.GetComponent<AudioPlayerUnit>();
                unit.source = source;

                return unit;
            }
            catch (Exception e)
            {
                Deb.LogException(e);
                return null;
            }
        }
        
        internal void Play(in AudioRefSO audioRef)
        {
            source.clip = audioRef.Clip;
            source.loop = audioRef.Loop;
            source.Play();
        }

        internal void PlayOneShot(in AudioRefSO audioRef)
        {
            source.PlayOneShot(audioRef.Clip, audioRef.Volume);
        }
    }
}