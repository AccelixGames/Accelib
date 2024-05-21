using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Accelib.Utility
{
    public class SimpleSceneChanger : MonoBehaviour
    {
        [SerializeField, Scene] private string targetScene;
        [SerializeField] private LoadSceneMode loadMode = LoadSceneMode.Single;

        private void OnEnable()
        {
#if UNITY_EDITOR
            if(SceneManager.loadedSceneCount == 1)
                SceneManager.LoadScene(targetScene,loadMode);
#else

                SceneManager.LoadScene(targetScene,loadMode);
#endif
        }
    }
}