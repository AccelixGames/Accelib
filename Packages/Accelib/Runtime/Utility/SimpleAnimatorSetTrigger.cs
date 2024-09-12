using System;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Utility
{
    
    [RequireComponent(typeof(Animator))]
    public class SimpleAnimatorSetTrigger : MonoBehaviour
    {
        [SerializeField, AnimatorParam(nameof(_animator), AnimatorControllerParameterType.Trigger)] private string triggerName;
        
        private Animator _animator;

        private void Awake() => _animator = GetComponent<Animator>();

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void SetTrigger()=> _animator.SetTrigger(triggerName);
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void ResetTrigger() => _animator.ResetTrigger(triggerName);

#if UNITY_EDITOR
        private void OnValidate() => _animator = GetComponent<Animator>();
#endif
    }
}