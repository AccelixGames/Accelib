using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Accelib.Utility
{
    public class SimpleSceneChanger : MonoBehaviour
    {
        private enum SceneType
        {
            Name = 0,
            Index
        }

        [Header("LoadScn")]
        [SerializeField]  private SceneType sceneType = SceneType.Name;
        [ShowIf(nameof(sceneType), SceneType.Name), SerializeField, Scene] private string targetScene;
        [ShowIf(nameof(sceneType), SceneType.Index), SerializeField, Scene] private int targetIndex;
        
        [Header("LoadMode")]
        [SerializeField] private LoadSceneMode loadMode = LoadSceneMode.Single;
        [SerializeField, Range(0, 128)] private int frameDelay = 0;

        private void Start() => Load().Forget();

        private async UniTaskVoid Load()
        {
            await UniTask.DelayFrame(frameDelay);
            
            if(sceneType == SceneType.Name)
                SceneManager.LoadScene(targetScene, loadMode);
            else
                SceneManager.LoadScene(targetIndex, loadMode);
        }
    }
}