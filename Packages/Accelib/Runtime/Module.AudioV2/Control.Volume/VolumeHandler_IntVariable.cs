using System;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Audio;

namespace Accelib.Module.AudioV2.Control.Volume
{
    [DefaultExecutionOrder(-1000)]
    public class VolumeHandler_IntVariable : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup mixerGroup;
        [SerializeField] private IntVariable volumeVar;

        private void Start() => OnChanged(volumeVar.Value);

        public void OnEnable() => volumeVar.Changed.Register(OnChanged);

        public void OnDisable() => volumeVar.Changed.Unregister(OnChanged);

        private void OnChanged(int value)
        {
            var key = mixerGroup.name;
            var normal = Mathf.Clamp(value / 100f, 0.0001f, 1f);
            var volume = NormalToVolume(normal);
            
            mixerGroup.audioMixer.SetFloat(key, volume);
        }

        private float NormalToVolume(float value) => Mathf.Log10(value) * 20;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField, ReadOnly] private string mixerName;
        
        private void OnValidate()
        {
            mixerName = mixerGroup?.name;
        }
#endif
    }
}