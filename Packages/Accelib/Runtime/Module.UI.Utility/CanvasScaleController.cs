using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.UI;

namespace Accelib.Module.UI.Utility
{
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasScaleController : MonoBehaviour
    {
        [SerializeField] private FloatVariable interfaceScale;
        [SerializeField, MinMaxSlider(1f, 2f)] private Vector2 range = new(1f, 2f);
        [SerializeField, ReadOnly] private float referenceY = 0f;
        [SerializeField, ReadOnly] private float currY;
        
        private CanvasScaler _canvasScaler;

        private void Awake()
        {
            _canvasScaler = GetComponent<CanvasScaler>();
            referenceY = _canvasScaler.referenceResolution.y;
        }

        private void OnEnable()
        {
            if (_canvasScaler == null) return;
            
            interfaceScale?.Changed.Register(OnChanged);
            OnChanged(interfaceScale?.Value ?? 1f);
        }

        private void OnDisable() => interfaceScale?.Changed.Unregister(OnChanged);
        
        private void OnChanged(float scl)
        {
            scl = Mathf.Clamp(scl, range.x, range.y);
            currY = referenceY * scl;

            var res = _canvasScaler.referenceResolution;
            res.y = currY;
            _canvasScaler.referenceResolution = res;
        }
    }
}