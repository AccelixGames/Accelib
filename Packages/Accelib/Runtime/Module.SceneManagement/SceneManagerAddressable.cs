using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Module.Transition;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
// ReSharper disable PossibleMultipleEnumeration

namespace Accelib.Module.SceneManagement
{
    public static class SceneManagerAddressable
    {
        private static SO_SceneConfig Config => SO_SceneConfig.Instance;

        public static UniTask<bool> ChangeScnPreviousAsync(int transitionIndex = 0) => ChangeScnAsync(Config.prevScn, transitionIndex);

        public static async UniTask<bool> ChangeScnAsync(AssetReference scnRef, int transitionIndex = 0)
        {
            // 로딩 우선도 캐싱
            var defaultLoadPriority = Application.backgroundLoadingPriority;
            
            try
            {
                // 에러처리
                if (scnRef == null) throw new Exception("변경하려는 씬이 NULL 입니다.");
                
                // 잠금 처리
                if (Config.isLocked) throw new Exception("씬이 이미 변경중입니다.");
                Config.isLocked = true;
                
                // 씬 미리 로드
                //Addressables.load
                //var scnDownloadTask = Addressables.DownloadDependenciesAsync(scnRef);
                
                // 트렌지션 시작
                if(transitionIndex >= 0)
                    await TransitionSingleton.StartTransition(transitionIndex);
                
                // 로딩 우선순위 변경
                if(Config.ChangeBackgroundLoadSpd)
                    Application.backgroundLoadingPriority = ThreadPriority.High;
                
                // 빈 씬으로 교체 (완전 교체)
                await SceneManager.LoadSceneAsync(Config.EmptyScnName, LoadSceneMode.Single);
                
                // 가비지 컬렉팅
                CollectGarbage();

                // 다운로드 대기 
                //await scnDownloadTask;
                var instance = await scnRef.LoadSceneAsync(LoadSceneMode.Single, true);
                Config.prevScn = Config.currScn;
                Config.currScn = scnRef;
                
                // 핸들러 가져오기
                var handlers = instance.Scene.GetRootGameObjects()
                    .SelectMany(o => o.GetComponentsInChildren<ISceneChangedEventHandler>(true))
                    .Where(h => h != null)
                    .ToArray();
                
                foreach (var handler in handlers) 
                    await handler.OnAfterSceneChanged();
            }
            catch (Exception e)
            {
                // 실패
                Debug.LogException(e);
                return false;
            }
            finally
            {
                // 마무리 - 로딩 우선도 복구
                if(Config.ChangeBackgroundLoadSpd)
                    Application.backgroundLoadingPriority = defaultLoadPriority;
                
                // 트렌지션 종료
                if(transitionIndex >= 0)
                    await TransitionSingleton.EndTransition();
                
                // 마무리 - 잠금 해제
                Config.isLocked = false;
            }
            
            // 성공
            return true;
        }
        
        public static async UniTask<bool> AddScnAsync(params AssetReference[] scnRefs)
        {
            try
            {
                // 잠금 처리
                if (Config.isLockedAdditive) 
                    throw new InvalidKeyException("씬이 이미 변경중입니다.");
                Config.isLockedAdditive = true;
                
                // 씬 로드 테스크 추가
                var tasks = new List<UniTask>();
                foreach (var scnRef in scnRefs)
                {
                    tasks.Add(scnRef.LoadSceneAsync(LoadSceneMode.Additive, true).ToUniTask());
                    // Debug.Log("Scene load reserved: " + scnRef.RuntimeKey, scnRef.editorAsset);
                }

                // 대기
                await tasks;
            }
            catch (Exception e)
            {
                // 실패
                Debug.LogException(e);
                return false;
            }
            finally
            {
                // 마무리 - 잠금 해제
                Config.isLockedAdditive = false;
            }
            
            // 성공
            return true;
        }
        
        public static async UniTask<bool> RemoveScnAsync(AssetReference[] scnRefs)
        {
            try
            {
                // 잠금 처리
                if (Config.isLocked) 
                    throw new InvalidKeyException("씬이 이미 변경중입니다.");
                Config.isLocked = true;
                
                // 씬 로드 테스크 추가
                var tasks = new List<UniTask>();
                foreach (var scnRef in scnRefs) 
                    tasks.Add(scnRef.UnLoadScene().ToUniTask());

                // 대기
                await tasks;
            }
            catch (Exception e)
            {
                // 실패
                Debug.LogException(e);
                return false;
            }
            finally
            {
                // 마무리 - 잠금 해제
                Config.isLocked = false;
            }
            
            // 성공
            return true;
        }
        
        private static void CollectGarbage()
        {
            Resources.UnloadUnusedAssets();
            
            if(Config.GCOnUnload)
                GC.Collect();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => SO_SceneConfig.Instance.Init();

    }
}