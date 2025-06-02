using UnityEngine;
using UnityEngine.Pool;

namespace Accelib.Module.ObjectPool
{
    public interface IPoolableGameObject
    {
        public IObjectPool<GameObject> Pool { get; set; }
        
        public void Release();
    }
}
