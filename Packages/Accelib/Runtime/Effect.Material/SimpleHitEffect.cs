using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Effect.Material
{
    public class SimpleHitEffect : MonoBehaviour
    {
        [SerializeField] private Renderer render;

        [Header("Effect")]
        [SerializeField] private string propertyName = "_HitEffectBlend";
        [SerializeField, Range(0.001f, 10f)] private float fadeDuration = 0.1f;
        [SerializeField, Range(0f, 10f)] private float keepDuration = 0.1f;
        [SerializeField, ReadOnly] private int propertyID;
        [SerializeField, ReadOnly] private float amount;

        private void Awake()
        {
            propertyID = Shader.PropertyToID(propertyName);
        }

        private Sequence DoHit()
        {
            var halfDuration = fadeDuration * 0.5f;
            return DOTween.Sequence()
                .Append(DOTween.To(() => amount, x => amount = x, 1f, halfDuration))
                .AppendInterval(keepDuration)
                .Append(DOTween.To(() => amount, x => amount = x, 0f, halfDuration))
                .OnUpdate(() => render.material.SetFloat(propertyID, amount));
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void Hit() => DoHit();

        private void Reset()
        {
            render = GetComponent<Renderer>();
        }
    }
}