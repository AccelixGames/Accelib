﻿using System;
using Accelib.Audio.Data;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Audio.Helper
{
    public class SimpleAudioPlayer : MonoBehaviour
    {
        private enum Mode
        {
            None,
            Play,
            PlayOneShot
        }

        [SerializeField] private Mode onEnableMode = Mode.None;
        [SerializeField] private AudioRefSO_Default audioRef;

        [Button(enabledMode: EButtonEnableMode.Playmode)] public void Play() => audioRef?.Play();
        [Button(enabledMode: EButtonEnableMode.Playmode)] public void PlayOneShot() => audioRef?.PlayOneShot();

        private void OnEnable()
        {
            if (onEnableMode == Mode.Play) Play();
            else if (onEnableMode == Mode.PlayOneShot) PlayOneShot();
        }

        private void Reset()
        {
            gameObject.name = "(SimpleAudioPlayer) Default";
        }
    }
}