using UnityEngine;
using UnityEngine.Pool;

namespace Accelib.Module.ObjectPool
{
    public class GameObjectPool : MonoBehaviour
    {
        [Header("Pool Object")]
        [SerializeField] private GameObject prefab;
        [SerializeField] public int defaultCapacity = 10;
        [SerializeField] public int maxPoolSize = 15;
        
        public IObjectPool<GameObject> Pool { get; private set; }

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                OnDestroyPoolObject, true, defaultCapacity, maxPoolSize);

            // 오브젝트 생성
            for (var i = 0; i < defaultCapacity; i++)
            {   
                var go = CreatePooledItem();
                if (go.TryGetComponent(out IPoolableGameObject poolObj))
                {
                    poolObj.Pool = Pool;
                    poolObj.Pool.Release(go.gameObject);
                    
                    go.name = go.name.Replace("(Clone)", $"{i}");
                }
            }
        }
        
        // 생성
        private GameObject CreatePooledItem()
        {
            var poolGo = Instantiate(prefab, transform);
            return poolGo;
        }

        // 사용
        private void OnTakeFromPool(GameObject poolGo)
        {
            poolGo.SetActive(true);
        }

        // 반환
        private void OnReturnedToPool(GameObject poolGo)
        {
            poolGo.SetActive(false);
        }

        // 삭제
        private void OnDestroyPoolObject(GameObject poolGo)
        {
            Destroy(poolGo);
        }
        
        /// <summary>
        /// 풀에 있는 오브젝트 가져오기
        /// </summary>
        /// <returns></returns>
        public GameObject GetPooledObject()
        {
            return Pool?.Get();
        }
    }
}
