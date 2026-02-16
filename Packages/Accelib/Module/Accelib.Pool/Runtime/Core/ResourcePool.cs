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

        /// <summary>새 인스턴스를 생성한다. 풀이 비었을 때 호출된다.</summary>
        protected abstract T New();

        /// <summary>풀에서 객체를 꺼낸다. 풀이 비어 있으면 <see cref="New"/>로 생성한다.</summary>
        public T Get()
        {
            // 스택에서 꺼내기 시도, 없으면 새로 생성
            if (!_stack.TryPop(out var resource))
                return New();

            size -= 1;
            return resource;
        }

        /// <summary>객체를 풀에 반환한다. <see cref="IPoolTarget.OnRelease"/>를 호출한 뒤 스택에 저장한다.</summary>
        public void Dispose(T resource)
        {
            // 상태 초기화 후 스택에 반환
            resource.OnRelease();
            _stack.Push(resource);
            size += 1;
        }

        /// <summary>객체를 완전히 파괴할 때 호출된다. <see cref="DestroyAll"/>에서 사용한다.</summary>
        protected abstract void OnDestroy(T resource);

        /// <summary>풀 내 모든 객체를 파괴하고 풀을 비운다.</summary>
        public void DestroyAll()
        {
            // 스택 내 모든 객체 파괴
            foreach (var target in _stack)
                OnDestroy(target);

            // 스택 초기화
            _stack.Clear();
            size = 0;
        }
    }
}
