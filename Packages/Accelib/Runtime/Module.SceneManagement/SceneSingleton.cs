using System;
using Accelib.Core;
using Accelib.Module.Transition;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Accelib.Module.SceneManagement
{
    public class SceneSingleton : MonoSingleton<SceneSingleton>
    {
        [AssetSelector(Filter = "t:Scene")]
        [SerializeField] private string emptyScene;
        
        [Header("Current")]
        [SerializeField, ReadOnly] private bool isLocked = false;
        
        private void OnEnable() => isLocked = false;
        private void OnDisable() => isLocked = false;
        
        public async UniTask<SceneInstance?> ChangeAsync(AssetReference sceneAsset)
        {
            // 잠금 체크 및 설정
            if (isLocked) return null;

            SceneInstance? result = null; 
            var defaultLoadPriority = Application.backgroundLoadingPriority;

            try
            {
                isLocked = true;
                
                // 캐싱
                var activeScn = SceneManager.GetActiveScene();

                // 로딩 우선순위 변경
                Application.backgroundLoadingPriority = ThreadPriority.High;
                
                // 빈 씬 로드
                await SceneManager.LoadSceneAsync(emptyScene, LoadSceneMode.Additive);

                // 이전 씬 언로드
                await SceneManager.UnloadSceneAsync(activeScn);

                // 가비지 컬렉팅
                GarbageCollect();

                // 씬 로드
                result = await sceneAsset.LoadSceneAsync(LoadSceneMode.Additive);
                SceneManager.SetActiveScene(result.Value.Scene);

                // 빈 씬 언로드
                SceneManager.UnloadSceneAsync(emptyScene);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                Application.backgroundLoadingPriority = defaultLoadPriority;
                isLocked = false;
            }
            
            return result;
        }
        
        private void GarbageCollect()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}