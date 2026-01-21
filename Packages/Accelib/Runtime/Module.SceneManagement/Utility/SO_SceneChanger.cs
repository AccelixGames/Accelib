using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Accelib.Module.SceneManagement.Utility
{
    [CreateAssetMenu(fileName = "SceneChanger", menuName = "Accelib/SceneChanger", order = 10)]
    public class SO_SceneChanger : ScriptableObject
    {
        [Title("옵션")]
        [SerializeField] private LoadSceneMode loadMode = LoadSceneMode.Single;
        
        [ShowIf("loadMode", LoadSceneMode.Single)]
        [SerializeField] private int transitionIndex = 0;
        [ShowIf("loadMode", LoadSceneMode.Single)]
        [SerializeField] private AssetReference sceneAsset;
        [ShowIf("loadMode", LoadSceneMode.Additive)]
        [SerializeField] private AssetReference[] sceneAssets;
        
        [Button]
        public void Change() => SceneManagerAddressable.ChangeScnAsync(sceneAsset, transitionIndex).Forget();
        
        [Button]
        public void AddScenes() => SceneManagerAddressable.AddScnAsync(sceneAssets).Forget();
        [Button]
        public void RemoveScenes() => SceneManagerAddressable.RemoveScnAsync(sceneAssets).Forget();
    }
}