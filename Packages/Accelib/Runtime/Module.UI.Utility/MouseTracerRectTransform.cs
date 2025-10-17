using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Accelib.Module.UI.Utility
{
    public class MouseTracerRectTransform : MonoBehaviour
    {
        [Header("Mouse Tracer")]
        [SerializeField] private bool enableScreenSafe = false;
        [SerializeField] private RectTransform pivot;
        [SerializeField] private Vector2 offset = Vector2.zero;
        
        [Header("Values")]
        [SerializeField, ReadOnly] private Vector2 screenHalf;

        private Mouse _mouse;
        
        private void OnEnable()
        {
            _mouse = Mouse.current;
        }

        private void LateUpdate()
        {
            if (!enabled) return;
            if (!pivot) return;

            var mousePos = _mouse.position.value;
            screenHalf = new Vector2(Screen.width, Screen.height) * 0.5f;
            var pivotPos = Vector2.zero;
            var finalOffset = offset;
        
            if (enableScreenSafe)
            {
                var isRight = mousePos.x >= screenHalf.x;
                pivotPos.x = isRight  ? 1f : 0f;
                finalOffset.x = offset.x * (isRight ? -1f : 1f);
                
                var isUp = mousePos.y >= screenHalf.y;
                pivotPos.y = isUp ? 1f : 0f;
                finalOffset.y = offset.y * (isUp ? 1f : -1f);
        
                pivot.pivot = pivotPos;
            }
            
            //pivot.anchoredPosition = finalOffset + mousePos;
            pivot.position = finalOffset + mousePos;
        }
    }
}