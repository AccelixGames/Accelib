using Accelib.Logging;
using UnityAtoms;
using UnityEngine;

namespace Accelib.Module.Prefs.Data.Base
{
    [System.Serializable]
    public abstract class PrefsVar
    {
        protected AtomEventBase EventBase;
        [SerializeField, ReadOnly] private bool _isDirty = false; 
        
        public void OnStart()
        {
            EventBase.Register(SetDirty);
            Internal_Read();
            _isDirty = false;
        }

        public void OnDestroy() => EventBase.Unregister(SetDirty);
        
        public bool Write()
        {
            if(!_isDirty) return false;
            
            _isDirty = false;
            Internal_Write();
            return true;
        }

        protected abstract void Internal_Read();
        
        protected abstract void Internal_Write();
        
        private void SetDirty() => _isDirty = true;
    }
}