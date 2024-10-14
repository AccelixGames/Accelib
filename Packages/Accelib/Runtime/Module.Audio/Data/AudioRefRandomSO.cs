using System;
using Accelib.Logging;
using Accelib.Module.Audio.Data._Base;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Accelib.Module.Audio.Data
{
    [CreateAssetMenu(fileName = "(AudioRefRandom) Name", menuName = "Accelib/AudioRefRandom", order = 0)]
    public class AudioRefRandomSO : AudioRefBase
    {
        [Header("Resource")]
        [SerializeField] private AudioChannel channel;
        [SerializeField] public AudioClip[] clips;

        [Header("Option")]
        [SerializeField, Range(0f, 1f)] private float volume;
        [SerializeField] public bool loop = false;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool showLog = false;
        public override bool ShowLog => showLog;
#endif
        
        public override AudioChannel Channel => channel;
        public override AudioClip Clip => clips is { Length: > 0 } ? clips[Random.Range(0, clips.Length)] : null;
        public override float Volume => volume;
        public override bool Loop => loop;
        protected override bool Validate() => Clip;

    }
}