using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Pool
{
    /// <summary>
    /// Stack 기반 제네릭 리소스 풀. MonoBehaviour가 아닌 일반 객체 풀링에 사용한다.
    /// </summary>
    [System.Serializable]
    public abstract class ResourcePool<T> where T : IPoolTarget, new()
    {
        [Header("# 풀 정보")]
        [SerializeField, ReadOnly] private int size = 0;

        private readonly Stack<T> _stack = new();

        protected abstract T New();

        public T Get()
        {
            if (!_stack.TryPop(out var resource))
                return New();

            size -= 1;
            return resource;
        }

        public void Dispose(T resource)
        {
            resource.OnRelease();
            _stack.Push(resource);
            size += 1;
        }

        protected abstract void OnDestroy(T resource);

        public void DestroyAll()
        {
            foreach (var target in _stack)
                OnDestroy(target);

            _stack.Clear();
            size = 0;
        }
    }
}
