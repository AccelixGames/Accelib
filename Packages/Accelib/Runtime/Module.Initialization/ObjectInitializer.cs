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
        [SerializeField, Scene] private string targetScene;
        
        [field: Header("State")]
        [field: SerializeField, ReadOnly] public State InitState { get; private set; }

        private async void Start()
        {
            InitState = State.None;
            
            var tasks = new List<UniTask<bool>>();
            foreach (var go in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                if (go == null) continue;

                if (go.TryGetComponent<IInitRequired>(out var initRequired))
                    initRequired.Initialize();

                if (go.TryGetComponent<IAsyncInitRequired>(out var asyncInitRequired))
                    tasks.Add(asyncInitRequired.InitAsync());
            }

            var results = await UniTask.WhenAll(tasks);
            InitState = results.All(isTrue => isTrue) ? State.Success : State.Failed;

            if (loadScnAfterInit)
                SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
        }
    }
}