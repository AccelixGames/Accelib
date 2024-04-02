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

        public void Unload()
        {
            if(handler == null)
                Debug.LogError($"{gameObject.name}: ModuleHandler가 없어서 Unload 불가합니다.", this);
            else
                handler.Unload();
        }
    }
}
