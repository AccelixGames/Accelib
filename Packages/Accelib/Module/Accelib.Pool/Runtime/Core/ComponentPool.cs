using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Accelib.Pool
{
    /// <summary>
    /// List 기반 델리게이트 구동 컴포넌트 풀. 생성/풀링/반환 콜백을 외부에서 주입한다.
    /// </summary>
    [Serializable]
    public class ComponentPool<T> where T : class
    {
        [ShowInInspector] protected List<T> _releasedList = new();
        public IReadOnlyList<T> ReleasedList => _releasedList;

        public Func<T> New = null;
        public Action<T> OnPooled = null;
        public Action<T> OnReleased = null;

        /// <summary>풀에서 객체를 꺼낸다. 풀이 비어 있으면 <see cref="New"/>로 생성한다.</summary>
        public T Get()
        {
            T pooled;

            // 풀에서 꺼내기 시도, 없으면 새로 생성
            if (_releasedList.Count <= 0)
            {
                pooled = New?.Invoke();
            }
            else
            {
                var index = _releasedList.Count - 1;
                pooled = _releasedList[index];
                _releasedList.RemoveAt(index);
            }

            // 꺼낸 후 콜백 호출
            OnPooled?.Invoke(pooled);
            return pooled;
        }

        /// <summary>객체를 풀에 반환한다. 중복 반환은 무시된다.</summary>
        public void Release(T comp)
        {
            if (!_releasedList.Contains(comp))
            {
                OnReleased?.Invoke(comp);
                _releasedList.Add(comp);
            }
        }
    }
}
