using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Accelib.Module.UI.Utility
{
    public class EventSystem_Select : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        [SerializeField] private LifeCycleType type;

        [Header("Option")]
        [SerializeField] private bool restorePreviousOnDisabled;
        [SerializeField, ReadOnly] private GameObject previous;

        [Button]
        public async UniTaskVoid Select()
        {
            previous = EventSystem.current.currentSelectedGameObject;
            await UniTask.DelayFrame(1);
            EventSystem.current.SetSelectedGameObject(target);
        }
        
        private void Awake()
        {
            if (type == LifeCycleType.Awake) Select().Forget();
        }

        private void Start()
        {
            if (type == LifeCycleType.Start) Select().Forget();
        }

        private void OnEnable()
        {
            if (type == LifeCycleType.OnEnable) Select().Forget();
        }

        private void OnDisable()
        {
            if (type == LifeCycleType.OnDisable) Select().Forget();
            else if (restorePreviousOnDisabled && previous != null) EventSystem.current.SetSelectedGameObject(previous);
        }

        private void OnDestroy()
        {
            if (type == LifeCycleType.OnDestroy) Select().Forget();
        }

        private void Reset()
        {
            target = gameObject;
        }
    }
}