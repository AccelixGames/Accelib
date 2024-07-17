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

        private RectTransform _rt;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
        }

        private void Start()
        {
            timer = 0f;
            localPos = _rt == null ? transform.localPosition : _rt.anchoredPosition;
        }

        private void OnEnable()
        {
            timer = 0f;
            
            localPos.y = curve.Evaluate(0f) * height;
            if (_rt == null)
                transform.localPosition = localPos;
            else
                _rt.anchoredPosition = localPos;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= duration * 2f)
                timer -= duration * 2f;

            var normal = timer / duration;
            var eval = curve.Evaluate(normal);

            localPos.y = eval * height;
            if (_rt == null)
                transform.localPosition = localPos;
            else
                _rt.anchoredPosition = localPos;
        }
    }
}