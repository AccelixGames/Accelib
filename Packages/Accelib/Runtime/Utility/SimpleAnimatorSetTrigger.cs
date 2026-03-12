using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Utility
{
    
    [RequireComponent(typeof(Animator))]
    public class SimpleAnimatorSetTrigger : MonoBehaviour
    {
        [SerializeField] private string triggerName;
        [SerializeField] private bool resetTriggerOnEnable;
        
        private Animator _animator;

        private void Awake() => _animator = GetComponent<Animator>();

        private void OnEnable()
        {
            if(resetTriggerOnEnable) ResetTrigger();
        }

        [Button, EnableIf("@UnityEngine.Application.isPlaying")]
        public void SetTrigger()=> _animator.SetTrigger(triggerName);
        [Button, EnableIf("@UnityEngine.Application.isPlaying")]
        public void ResetTrigger() => _animator.ResetTrigger(triggerName);

#if UNITY_EDITOR
        private void OnValidate() => _animator = GetComponent<Animator>();
#endif
    }
}