using System;
using Accelib.Tween;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Accelib.Effect
{
    public class SimpleFloatingEffect : MonoBehaviour
    {
        [SerializeField] private float height;
        [SerializeField] private float duration;
        [SerializeField] private AnimationCurve curve;

        [Header("")]
        [SerializeField, ReadOnly] private float timer;
        [SerializeField, ReadOnly] private Vector3 localPos;

        private void Start()
        {
            timer = 0f;
            localPos = transform.localPosition;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= duration * 2f)
                timer -= duration * 2f;

            var normal = timer / duration;
            var eval = curve.Evaluate(normal);

            localPos.y = eval * height;
            transform.localPosition = localPos;
        }
    }
}