using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimpleFloatingEffect : MonoBehaviour
    {
        [SerializeField] private float height;
        [SerializeField] private float duration;
        [SerializeField, Range(0f, 1f)] private float offset = 0f;
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
            var eval = curve.Evaluate(normal + offset);

            localPos.y = eval * height;
            if (_rt == null)
                transform.localPosition = localPos;
            else
                _rt.anchoredPosition = localPos;
        }
    }
}