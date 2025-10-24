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
    }
}