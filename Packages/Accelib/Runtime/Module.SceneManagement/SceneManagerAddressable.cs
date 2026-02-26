using System;
using System.Collections.Generic;
using System.Linq;
using Accelib.Module.Transition;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
// ReSharper disable PossibleMultipleEnumeration

namespace Accelib.Module.SceneManagement
{
    public static class SceneManagerAddressable
    {
        private static SO_SceneConfig Config => SO_SceneConfig.Instance;

        public static UniTask<SceneInstance?> ChangeScnPreviousAsync(int transitionIndex = 0) =>
            ChangeScnAsync(Config.prevScn, transitionIndex);

        public static async UniTask<SceneInstance?> ChangeScnAsync(AssetReference scnRef, int transitionIndex = 0)
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

                // 트렌지션 시작
                if (transitionIndex >= 0)
                    await TransitionSingleton.StartTransition(transitionIndex);

                // 로딩 우선순위 변경
                if (Config.ChangeBackgroundLoadSpd)
                    Application.backgroundLoadingPriority = ThreadPriority.High;

                // 빈 씬으로 교체 (완전 교체)
                await SceneManager.LoadSceneAsync(Config.EmptyScnName, LoadSceneMode.Single);
                
                // 오디오 켜기
                TransitionSingleton.Instance.ToggleAudioListener(true);

                // 가비지 컬렉팅
                CollectGarbage();

                // 다운로드 대기 
                var instance = await scnRef.LoadSceneAsync();
                Config.prevScn = Config.currScn;
                Config.currScn = scnRef;

                // 오디오 끄기
                TransitionSingleton.Instance.ToggleAudioListener(false);
                
                // 핸들러 가져오기
                var handlers = instance.Scene.GetRootGameObjects()
                    .SelectMany(o => o.GetComponentsInChildren<ISceneChangedEventHandler>(true))
                    .Where(h => h != null)
                    .ToArray();

                // 이벤트
                foreach (var handler in handlers)
                    await handler.OnAfterSceneChanged();

                // 반환
                return instance;
            }
            catch (Exception e)
            {
                // 실패
                Debug.LogException(e);
                return null;
            }
            finally
            {
                // 마무리 - 로딩 우선도 복구
                if (Config.ChangeBackgroundLoadSpd)
                    Application.backgroundLoadingPriority = defaultLoadPriority;

                // 트렌지션 종료
                if (transitionIndex >= 0)
                    await TransitionSingleton.EndTransition();

                // 마무리 - 잠금 해제
                Config.isLocked = false;
            }
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
                    // null/빈 참조 건너뛰기
                    if (scnRef == null || !scnRef.RuntimeKeyIsValid())
                    {
                        Debug.LogError($"[AddScnAsync] null이거나 유효하지 않은 AssetReference가 포함되어 있습니다. (key: {scnRef?.RuntimeKey})");
                        continue;
                    }

                    // Stale 핸들 정리 (ChangeScnAsync의 Single 모드로 씬이 파괴되었지만 핸들이 남은 경우)
                    if (scnRef.OperationHandle.IsValid())
                    {
                        Debug.LogWarning($"[AddScnAsync] 이전 씬 핸들이 남아있어 해제합니다. (key: {scnRef.RuntimeKey})");
                        try { await scnRef.UnLoadScene().ToUniTask(); }
                        catch { /* 이미 파괴된 씬 — 핸들 해제만 수행 */ }
                    }

                    tasks.Add(scnRef.LoadSceneAsync(LoadSceneMode.Additive, true).ToUniTask());
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

                // 씬 언로드 테스크 추가
                var tasks = new List<UniTask>();
                foreach (var scnRef in scnRefs)
                {
                    if (scnRef == null || !scnRef.RuntimeKeyIsValid())
                    {
                        Debug.LogError($"[RemoveScnAsync] null이거나 유효하지 않은 AssetReference가 포함되어 있습니다. (key: {scnRef?.RuntimeKey})");
                        continue;
                    }

                    if (!scnRef.OperationHandle.IsValid())
                    {
                        Debug.LogWarning($"[RemoveScnAsync] 이미 해제된 씬 핸들입니다. 건너뜁니다. (key: {scnRef.RuntimeKey})");
                        continue;
                    }

                    tasks.Add(scnRef.UnLoadScene().ToUniTask());
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
                Config.isLocked = false;
            }

            // 성공
            return true;
        }

        private static void CollectGarbage()
        {
            Resources.UnloadUnusedAssets();

            if (Config.GCOnUnload)
                GC.Collect();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() => SO_SceneConfig.Instance.Init();
    }
}