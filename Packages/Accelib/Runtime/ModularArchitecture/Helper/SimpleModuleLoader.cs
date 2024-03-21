using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Accelib.ModularArchitecture.Helper
{
    internal class SimpleModuleLoader : MonoBehaviour
    {
        [SerializeField] private ModuleHandler targetModule;
        [SerializeField] private LoadSceneMode loadMode = LoadSceneMode.Single;

        private void OnEnable()
        {
            targetModule.Load(loadMode);
        }
    }
}