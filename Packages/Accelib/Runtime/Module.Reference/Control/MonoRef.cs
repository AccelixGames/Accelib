using Accelib.Module.Reference.Model;
using UnityEngine;

namespace Accelib.Module.Reference.Control
{
    public abstract class MonoRef<T> : MonoBehaviour where T : Component
    {
        [SerializeField] private SO_Ref<T> reference;

        private void Awake() => Internal_Awake(reference.Register(GetComponent<T>()));
        private void OnDestroy() => Internal_OnDestroy(reference.UnRegister(GetComponent<T>()));

        protected virtual void Internal_Awake(bool isRegistered) {}
        protected virtual void Internal_OnDestroy(bool isUnRegistered) {}
    }
}