using NaughtyAttributes;
using UnityEngine;

namespace Accelib.ModularArchitecture
{
    /// <summary>
    /// 모듈의 베이스 클래스
    /// </summary>
    public class ModuleBase : MonoBehaviour
    {
        [SerializeField, ReadOnly] internal ModuleHandler handler;

        public void Unload() => handler.Unload();
    }
}
