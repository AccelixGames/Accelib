using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Accelib.InputSystem
{
    [RequireComponent(typeof(Button))]
    public class InputSystem_ButtonShortCut : MonoBehaviour
    {
        [SerializeField] private InputActionReference actionRef; // InputActionAsset에서 드래그
        
        [Header("Debug")]
        [SerializeField, ReadOnly] private bool isPressed = false;
        private PointerEventData _pointerEventData;

        void OnEnable()
        {
            _pointerEventData = new PointerEventData(EventSystem.current)
                { button = PointerEventData.InputButton.Left };
            isPressed = false;
            
            actionRef.action.started += OnActionDown;
            actionRef.action.canceled += OnActionUp;
            actionRef.action.Enable();
        }
        
        void OnDisable()
        {
            _pointerEventData = null;
            isPressed = false;
            
            actionRef.action.started -= OnActionDown;
            actionRef.action.canceled -= OnActionUp;
            actionRef.action.Disable();
        }

        private void OnActionDown(InputAction.CallbackContext context)
        {
            isPressed = true;
            ExecuteEvents.Execute(gameObject, _pointerEventData, ExecuteEvents.pointerDownHandler);
        }

        private void OnActionUp(InputAction.CallbackContext context)
        {
            isPressed = false;
            
            ExecuteEvents.Execute(gameObject, _pointerEventData, ExecuteEvents.pointerClickHandler);
            ExecuteEvents.Execute(gameObject, _pointerEventData, ExecuteEvents.pointerUpHandler);
        }
    }
}