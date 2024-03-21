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
            SceneManager.LoadScene(targetScene,loadMode);
        }
    }
}