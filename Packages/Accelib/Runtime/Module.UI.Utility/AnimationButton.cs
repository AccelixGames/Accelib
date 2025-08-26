using Accelib.Helper;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Accelib.Module.UI.Utility
{
    [RequireComponent(typeof(Animator))]
    public class AnimationButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("상태")]
        [SerializeField] private bool isEnabled = true;
        [SerializeField, Range(0.01f, 10f)] private float clickDuration = 1f;
        [SerializeField, Range(0.01f, 5f)] private float speedMultiplier = 1f;
        
        [Header("파라메터")]
        [SerializeField, AnimatorParam(nameof(anim))] private string animBoolHover = "isHover";
        [SerializeField, AnimatorParam(nameof(anim))] private string animBoolClick = "isClicked";
        [SerializeField, AnimatorParam(nameof(anim))] private string animBoolDisabled = "isDisabled";
        [SerializeField, AnimatorParam(nameof(anim))] private string animFloatSpeed = "speed";
        
        [Header("이벤트")]
        [SerializeField] public UnityEvent onPointerClick = new();
        [SerializeField] public UnityEvent onPointerHover = new();

        [Header("Debug")]
#pragma warning disable CS0219
        [SerializeField] private bool showDebValue = false;
        [SerializeField, ReadOnly, ShowIf(nameof(showDebValue))] private Animator anim;
        [SerializeField, ReadOnly, ShowIf(nameof(showDebValue))] private Timer timer;
        [SerializeField, ReadOnly, ShowIf(nameof(showDebValue))] private int paramHover;
        [SerializeField, ReadOnly, ShowIf(nameof(showDebValue))] private int paramClick;
        [SerializeField, ReadOnly, ShowIf(nameof(showDebValue))] private int paramDisabled;
        [SerializeField, ReadOnly, ShowIf(nameof(showDebValue))] private int pramSpeed;

        public bool IsEnabled => isEnabled;
        
        public void SetEnabled(bool enable)
        {
            if(isEnabled == enable) return;
            //if(!gameObject.activeSelf) return;
            
            isEnabled = enable;
            anim.SetBool(paramHover, false);
            anim.SetBool(paramClick, false);
            anim.SetBool(paramDisabled, !enable);
        }

        [Button]
        public void ToggleEnable() => SetEnabled(!isEnabled);
        
        private void Awake()
        {
            anim = GetComponent<Animator>();
            
            paramHover = Animator.StringToHash(animBoolHover);
            paramClick = Animator.StringToHash(animBoolClick);
            paramDisabled = Animator.StringToHash(animBoolDisabled);
            pramSpeed = Animator.StringToHash(animFloatSpeed);
        }

        private void OnEnable()
        {
            anim.Rebind();
            anim.SetBool(paramHover, false);
            anim.SetBool(paramClick, false);
            anim.SetBool(paramDisabled, !isEnabled);
            anim.SetFloat(pramSpeed, speedMultiplier);
        }

        private void OnDisable()
        {
            anim.SetBool(paramHover, false);
            anim.SetBool(paramClick, false);
            anim.StopPlayback();
            anim.Rebind();
        }

        private void Update()
        {
            if (isEnabled && anim.GetBool(paramClick) && timer.OnTime()) 
                anim.SetBool(paramClick, false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!isEnabled) return;
            
            if(!anim.GetBool(paramClick))
                anim.SetBool(paramHover, true);
            
            onPointerHover?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(!isEnabled) return;
            
            if(!anim.GetBool(paramClick))
                anim.SetBool(paramHover, false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
            if (!isEnabled) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (anim.GetBool(paramClick)) return;
            
            anim.SetBool(paramHover, false);
            anim.SetBool(paramClick, true);
            
            timer.Set(clickDuration / speedMultiplier);
            
            onPointerClick?.Invoke();
        }

#if UNITY_EDITOR
        private void Reset() => anim = GetComponent<Animator>();
#endif
    }
}
