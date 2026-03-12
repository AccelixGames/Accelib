using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Effect
{
    public class CameraShakeEffect : MonoBehaviour
    {
        [SerializeField] private float duration = 1f;
        [SerializeField] private float strength = 1f;
        [SerializeField] private int vibrato = 30;
        [SerializeField] private float randomness = 90f;
        [SerializeField] private bool fadeOut = true;
        [SerializeField] private ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full;

        [Header("Debug")] 
        [SerializeField, ReadOnly] private Camera cam;

        [ContextMenu(nameof(DoShake))]
        public void DoShake()
        {
            cam = Camera.main;
            if(cam == null) return;

            cam.DOShakePosition(duration, strength, vibrato, randomness, fadeOut, randomnessMode);
        }

        private void Reset()
        {
            duration = 1f;
            strength = 1f;
            vibrato = 30;
            randomness = 90f;
            fadeOut = true;
            randomnessMode = ShakeRandomnessMode.Full;
        }
    }
}