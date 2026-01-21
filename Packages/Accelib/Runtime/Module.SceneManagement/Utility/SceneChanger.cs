using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Accelib.Module.SceneManagement.Utility
{
    public class SceneChanger : MonoBehaviour
    {
        [Title("옵션")]
        [SerializeField] private LoadSceneMode loadMode = LoadSceneMode.Single;
        
        [ShowIf("loadMode", LoadSceneMode.Single)]
        [SerializeField] private AssetReference sceneAsset;
        [ShowIf("loadMode", LoadSceneMode.Additive)]
        [SerializeField] private AssetReference[] sceneAssets;
        
        [TitleGroup("Change")]
        [Button] public void ChangeScene() => SceneManagerAddressable.ChangeScnAsync(sceneAsset).Forget();
        [TitleGroup("Change")]
        [Button] public void ChangeScenePrevious() => SceneManagerAddressable.ChangeScnPreviousAsync().Forget();
        
        [TitleGroup("Add-Remove")]
        [Button] public void AddScenes() => SceneManagerAddressable.AddScnAsync(sceneAssets).Forget();
        [TitleGroup("Add-Remove")]
        [Button] public void RemoveScenes() => SceneManagerAddressable.RemoveScnAsync(sceneAssets).Forget();
    }
}