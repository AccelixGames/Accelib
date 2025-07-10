using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Effect.Material
{
    public class SimpleHitEffect : MonoBehaviour
    {
        [SerializeField] private Renderer render;

        [Header("Hit Effect")]
        [SerializeField] private string propertyName = "_HitEffectBlend";
        [SerializeField, Range(0f, 1f)] private float maxBlend = 1f; 
        [SerializeField, Range(0.001f, 10f)] private float initDuration = 0f;
        [SerializeField, Range(0f, 10f)] private float keepDuration = 0.1f;
        [SerializeField, Range(0.001f, 10f)] private float outDuration = 0.1f;
        [SerializeField, ReadOnly] private int propertyID;
        [SerializeField, ReadOnly] private float amount;

        protected Renderer Render => render;
        private Sequence _seq;
        
        protected virtual void Awake()
        {
            propertyID = Shader.PropertyToID(propertyName);
        }

        public Sequence DoHit()
        {
            _seq?.Kill();
            _seq = DOTween.Sequence()
                .OnStart(() =>
                {
                    amount = 0f;
                    render.material.SetFloat(propertyID, amount);
                })
                .Append(DOTween.To(() => amount, x => amount = x, maxBlend, initDuration))
                .AppendInterval(keepDuration)
                .Append(DOTween.To(() => amount, x => amount = x, 0f, outDuration))
                .OnUpdate(() => render.material.SetFloat(propertyID, amount));

            return _seq;
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void Hit() => DoHit();

        private void Reset()
        {
            render = GetComponent<Renderer>();
        }
    }
}