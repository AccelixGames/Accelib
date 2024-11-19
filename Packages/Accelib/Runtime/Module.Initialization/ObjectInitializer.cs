using System.Collections.Generic;
using System.Linq;
using Accelib.Module.Initialization.Base;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Accelib.Module.Initialization
{
    public class ObjectInitializer : MonoBehaviour
    {
        public enum State {None, Success, Failed}

        [Header("LoadScn")]
        [SerializeField] private bool loadScnAfterInit = true; 
        [ShowIf(nameof(loadScnAfterInit)),SerializeField, Scene] private string targetScene;
        
        [field: Header("State")]
        [field: SerializeField, ReadOnly] public State InitState { get; private set; }

        private async void Awake()
        {
            InitState = State.None;
            
            var tasks = new List<UniTask<bool>>();
            var monoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            foreach (var go in monoBehaviours)
            {
                if (go == null) continue;

                if (go.TryGetComponent<IInitRequired>(out var initRequired))
                    initRequired.Init();
                
                if (go.TryGetComponent<IAsyncInitRequired>(out var asyncInitRequired))
                    tasks.Add(asyncInitRequired.InitAsync());
            }

            var results = await UniTask.WhenAll(tasks);
            InitState = results.All(isTrue => isTrue) ? State.Success : State.Failed;

            var lateInits = new List<ILateInitRequired>();
            foreach (var go in monoBehaviours)
            {
                if (go == null) continue;
                
                if (go.TryGetComponent<ILateInitRequired>(out var initRequired))
                    lateInits.Add(initRequired);
            }
            
            lateInits.Sort((a,b)=>b.Priority.CompareTo(a.Priority));
            foreach (var lateInitRequired in lateInits) 
                lateInitRequired.Init();

            if (loadScnAfterInit)
                SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
        }
    }
}