using System;
using Accelib.ModularArchitecture;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Accelix.ModularArchitecture._Test
{
    public class TestModuleSandbox : MonoBehaviour
    {
        [SerializeField] private ModuleHandler handler;

        [Button] private void Load() => handler.Load(LoadSceneMode.Additive);
        [Button] private void Unload() => handler.Unload();
        
        private void OnEnable()
        {
            handler.OnLoaded += OnLoad;
            handler.OnUnloaded += OnUnLoad;
        }

        private void OnLoad() => Debug.Log("로드!");
        private void OnUnLoad() => Debug.Log("언로드!");
    }
}