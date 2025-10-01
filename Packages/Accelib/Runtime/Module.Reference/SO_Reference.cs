using System;
using Accelib.Logging;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.Reference
{
    public abstract class SO_Reference<T> : ScriptableObject where T : Component
    {
        [SerializeField, ReadOnly] private T reference;

        private void OnEnable() => reference = null;
        private void OnDisable() => reference = null;

        public bool Register(T target)
        {
            if (reference && reference != target)
            {
                Deb.LogError($"이미 레퍼런스가 등록된 객체입니다({reference.name})", reference);
                return false;
            }
            
            reference = target;
            return true;
        }
        
        public bool UnRegister(T target)
        {
            if (reference != target)
            {
                // Deb.LogError($"자신의 레퍼런스만 취소할 수 있습니다({reference?.name})", reference);
                return false;
            }

            reference = null;
            return true;
        }
        
        public T Reference => reference;

        public bool TryGetReference(out T target)
        {
            target = reference;
            return reference;
        }
    }
}