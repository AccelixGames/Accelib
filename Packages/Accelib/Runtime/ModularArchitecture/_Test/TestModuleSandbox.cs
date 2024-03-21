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
    }
}