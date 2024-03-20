using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Accelib.UI
{
    public class SimpleJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        [SerializeField] private RectTransform joystickOuter;
        [SerializeField] private RectTransform joystickInner;

        [field: Header("결과")] 
        [field: SerializeField, ReadOnly] public Vector2 Output { get; private set; }
        [field: SerializeField, ReadOnly] public Vector2 OutputNormalized { get; private set; }

        [Header("디버그")] 
        [SerializeField, ReadOnly] private bool isDown = false;
        
        private RectTransform rt;
        
        private Vector2 pointerPos;
        private float joystickRadius;
        private float joystickRadiusReversed;

        public void Clear()
        {
            pointerPos = joystickOuter.anchoredPosition;
            isDown = false;
            Output = Vector2.zero;
            OutputNormalized = Vector2.zero;
        }
        
        private void Awake()
        {
            rt = GetComponent<RectTransform>();
        }

        private void Start()
        {
            joystickRadius = joystickOuter.sizeDelta.x * 0.5f;
            joystickRadiusReversed = 1f / joystickRadius;
            
            pointerPos = joystickOuter.anchoredPosition;
            isDown = false;
            Output = Vector2.zero;
            OutputNormalized = Vector2.zero;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDown = true;

            ConvertPos(eventData.position);
            UpdateJoystick();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDown = false;
            
            pointerPos = joystickOuter.anchoredPosition;
            UpdateJoystick();
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if(!isDown) return;
            
            ConvertPos(eventData.position);
            UpdateJoystick();
        }

        private void ConvertPos(Vector2 screenPoint) =>
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenPoint, Camera.main,
                out pointerPos);

        private void UpdateJoystick()
        {
            var diff = pointerPos - joystickOuter.anchoredPosition;
            var norm = diff.normalized;
            var magnitude = diff.magnitude;

            // Clamp
            magnitude = Mathf.Clamp(magnitude, -joystickRadius, joystickRadius);
            
            // Set Joystick
            joystickInner.anchoredPosition = norm * magnitude;
            
            // Set Output
            Output = norm * Mathf.Abs(magnitude * joystickRadiusReversed);
            OutputNormalized = norm;
        }
    }
}