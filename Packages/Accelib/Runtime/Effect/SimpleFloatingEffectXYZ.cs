using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Effect
{
    public class SimpleFloatingEffectXYZ : MonoBehaviour
    {
        [SerializeField] private Vector3 moveAmount = Vector3.zero;
        [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField, Range(0.01f, 60f)] private float duration = 1f;

        [Header("")]
        [SerializeField, ReadOnly] private float timer;
        [SerializeField, ReadOnly] private Vector3 initPos;
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
            initPos = localPos;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= duration * 2f)
                timer -= duration * 2f;

            var normal = timer / duration;
            var eval = curve.Evaluate(normal);

            localPos = initPos + eval * moveAmount;
            if (_rt == null)
                transform.localPosition = localPos;
            else
                _rt.anchoredPosition = localPos;
        }

#if UNITY_EDITOR
        [Button]
        private void SetPingPong()
        {
            curve.preWrapMode = WrapMode.PingPong;
            curve.postWrapMode = WrapMode.PingPong;
        }
#endif
    }
}