using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Core;
using Accelib.Module.Initialization.Base;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Accelib.Module.Initialization
{
    public class InitSingleton : MonoSingleton<InitSingleton>
    {
        public enum State {None, Success, Failed}
        
        [field: SerializeField] public State InitState { get; private set; }

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
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => Initialize();
#endif
    }
}