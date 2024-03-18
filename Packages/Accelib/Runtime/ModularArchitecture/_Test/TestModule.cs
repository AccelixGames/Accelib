using System;
using Accelib.ModularArchitecture;
using UnityEngine;

namespace Accelix.ModularArchitecture._Test
{
    public class TestModule : ModuleBase
    {
        private void Start()
        {
            Debug.LogWarning("테스트 모듈 시작");
        }

        private void OnEnable()
        {
            Debug.Log("Test Module Enabled");
        }
    }
}