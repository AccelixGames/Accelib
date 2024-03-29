﻿using System;
using Accelib.Transition;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Accelib.ModularArchitecture
{
    /// <summary>
    /// 모듈을 로드/언로드 할 수 있도록 도와주는 SO
    /// </summary>
    [CreateAssetMenu(fileName = "ModuleHandler-", menuName = "Accelix/ModuleHandler", order = 0)]
    public class ModuleHandler : ScriptableObject
    {
        private enum LoadState {Unloaded = 0, Loaded}
        
        [Header("Setting")]
        [SerializeField, Scene] private string moduleScn;
        
        [Header("State")]
        [SerializeField, ReadOnly] private LoadState loadState = LoadState.Unloaded;
        private ModuleBase moduleInstance;

        public UnityAction OnLoad;
        public UnityAction OnUnload;

        public void LoadAdditive() => Load(LoadSceneMode.Additive);
        public void LoadSingle() => Load(LoadSceneMode.Single);
        
        /// <summary>모듈을 로드한다</summary>
        public void Load(LoadSceneMode mode)
        {
#if UNITY_EDITOR
            // 플레이중이 아닌 상태에서 로드 못하도록 막기 (에디터만)
            if (!Application.isPlaying) return;
#endif
            // 이미 로드된 경우, 예외처리
            if (moduleInstance != null)
                Debug.LogWarning($"{moduleScn}이 이미 로드된 상태입니다. 더이상 로드할 수 없습니다.", this);
            else
                LoadAsync(mode).Forget();
        }

        private async UniTaskVoid LoadAsync(LoadSceneMode mode)
        {
            try
            {
                // 화면전환 시작
                if (mode == LoadSceneMode.Single)
                    await TransitionHandler.StartTransition();

                // 씬 로드
                await SceneManager.LoadSceneAsync(moduleScn, mode);

                // 씬 가져오기 (이름으로)
                var scn = SceneManager.GetSceneByName(moduleScn);

                // 씬이 없거나 올바르지 않을 경우, 예외처리
                if (!scn.IsValid()) throw new Exception($"{moduleScn}은 Invalid한 씬입니다.");
                if (!scn.isLoaded) throw new Exception($"{moduleScn}이 로드되지 않았습니다.");

                // 씬의 루트 오브젝트를 순회하며
                foreach (var root in scn.GetRootGameObjects())
                    // 모듈 클래스를 찾는다
                    if (root.TryGetComponent<ModuleBase>(out var module))
                    {
                        // 인스턴스 캐싱
                        moduleInstance = module;
                        loadState = LoadState.Loaded;
                        // 인스턴스 핸들러 설정
                        moduleInstance.handler = this;
                        // 종료
                        break;
                    }

                // 화면전환 종료
                if (mode == LoadSceneMode.Single)
                    await TransitionHandler.EndTransition();
                
                OnLoad?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }

        /// <summary>모듈을 언로드한다</summary>
        public void Unload()
        {
#if UNITY_EDITOR
            // 플레이중이 아닌 상태에서 로드 못하도록 막기 (에디터만)
            if (!Application.isPlaying) return;
#endif
            
            try
            {
                // 이미 언로드된 경우, 예외처리
                if (moduleInstance == null) throw new Exception($"{moduleScn}은 로드되지 않았기 때문에, 언로드할 수 없습니다.");

                // 씬 언로드
                SceneManager.UnloadSceneAsync(moduleScn).completed += _ =>
                {
                    // 인스턴스 비우기
                    moduleInstance = null;
                    loadState = LoadState.Unloaded;
                    
                    OnUnload?.Invoke();
                };
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        }
    }
}