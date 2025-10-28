using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Accelib.Module.Reference.Model
{
    public abstract class SO_Ref<T> : ScriptableObject where T : Component
    {
        public abstract T Reference { get; }

        public abstract bool Register(T target);
        public abstract bool UnRegister(T target);

        protected const int BaseOrder = 100;
        protected const string _M = "Accelix.Ref/";
        protected const string _F = "(Ref) ";


        private void OnEnable()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorSettings.enterPlayModeOptionsEnabled)
            {
                UnityEditor.EditorApplication.playModeStateChanged -= HandlePlayModeStateChange;
                UnityEditor.EditorApplication.playModeStateChanged += HandlePlayModeStateChange;
            }
#endif
            
            SetInitialValues();
        }

        private void OnDisable() => SetInitialValues();
        
#if UNITY_EDITOR
        private void HandlePlayModeStateChange(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingEditMode)
                SetInitialValues();
        }
#endif
        
        protected abstract void SetInitialValues();
    }
}