using System.Collections.Generic;
using Accelib.Module.Initialization.Base;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Accelib.Module.Initialization
{
    public class ObjectInitializer : MonoBehaviour
    {
        public enum State {None, Success}

        [Header("LoadEnd")]
        public UnityEvent onLoadEnd;
        
        [field: Header("State")]
        [field: SerializeField, ReadOnly] public State InitState { get; private set; }

        private List<IInitRequired> _inits;
        private List<ILateInitRequired> _lateInits;

        private void Awake()
        {
            InitState = State.None;
            var monoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            
            _inits = new List<IInitRequired>();
            foreach (var go in monoBehaviours)
            {
                if (go == null) continue;

                if (go.TryGetComponent<IInitRequired>(out var initRequired))
                {
                    _inits.Add(initRequired);
                    initRequired.Init();
                }
            }

            _lateInits = new List<ILateInitRequired>();
            foreach (var go in monoBehaviours)
            {
                if (go == null) continue;
                
                if (go.TryGetComponent<ILateInitRequired>(out var initRequired))
                    _lateInits.Add(initRequired);
            }
            
            _lateInits.Sort((a,b)=>b.Priority.CompareTo(a.Priority));
            foreach (var lateInitRequired in _lateInits) 
                lateInitRequired.Init();
        }

        private void Update()
        {
            if(InitState != State.None) return;
            
            foreach (var initRequired in _inits)
                if(!initRequired.IsInitialized()) return;
            
            foreach (var initRequired in _lateInits)
                if(!initRequired.IsInitialized()) return;

            InitState = State.Success;
            onLoadEnd?.Invoke();
        }
    }
}