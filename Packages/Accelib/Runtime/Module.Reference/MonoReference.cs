using UnityEngine;

namespace Accelib.Module.Reference
{
    public abstract class MonoReference<T> : MonoBehaviour where T : Component
    {
        [Header("# 레퍼런스")]
        [SerializeField] private SO_Reference<T> reference;

        protected virtual void Awake() => reference.Register(GetComponent<T>());
        protected virtual void OnDestroy() => reference.UnRegister(GetComponent<T>());
    }
}