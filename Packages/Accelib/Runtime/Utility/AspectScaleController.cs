using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Utility
{
    [RequireComponent(typeof(Camera))]
    public class AspectScaleController : MonoBehaviour
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private float referenceHeight = 568;
        
        [SerializeField, ReadOnly] private float initialSize = 11.5f;
        [SerializeField, ReadOnly] private float ratio;
        [SerializeField, ReadOnly] private Camera cam;
        
        private void Start()
        {
            cam = GetComponent<Camera>();
            initialSize = cam.orthographicSize;
            
            ratio = 1 / Mathf.Min(referenceHeight / target.sizeDelta.y, 1f);
            cam.orthographicSize = initialSize * ratio;
        }
    }
}