using System;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.UI.SafeArea
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaRectTransform : MonoBehaviour
    {
        [Header("Options")]
        [SerializeField] private bool ignoreX = false;
        [SerializeField] private bool ignoreY = false;
        
        [Header("Debug")]
        [SerializeField, ReadOnly] private RectTransform rt;
        [SerializeField, ReadOnly] private Vector2 defaultAnchorMin = Vector2.zero;
        [SerializeField, ReadOnly] private Vector2 defaultAnchorMax = Vector2.one;
        
        private void Awake()
        {
            rt = GetComponent<RectTransform>();
            defaultAnchorMin = rt.anchorMin;
            defaultAnchorMax = rt.anchorMax;
        }

        private void OnEnable()
        {
            var instance = SafeAreaSingleton.Instance;
            if (instance == null) return;
            
            instance.onSafeAreaUpdated.AddListener(OnSafeAreaUpdated);
            OnSafeAreaUpdated(instance.CurrAnchorMin, instance.CurrAnchorMax);
        }

        private void OnDisable()
        {
            SafeAreaSingleton.Instance?.onSafeAreaUpdated?.RemoveListener(OnSafeAreaUpdated);
        }

        private void OnSafeAreaUpdated(Vector2 anchorMin, Vector2 anchorMax)
        {
            anchorMin = Vector2.Max(defaultAnchorMin, anchorMin);
            anchorMax = Vector2.Min(defaultAnchorMax, anchorMax);

            if (ignoreX)
            {
                anchorMin.x = defaultAnchorMin.x;
                anchorMax.x = defaultAnchorMax.x;
            }

            if (ignoreY)
            {
                anchorMin.y = defaultAnchorMin.y;
                anchorMax.y = defaultAnchorMax.y;
            }

            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
        }
    }
}