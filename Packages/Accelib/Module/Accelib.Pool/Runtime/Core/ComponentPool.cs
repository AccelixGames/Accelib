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

        public T Get()
        {
            T pooled;
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

            OnPooled?.Invoke(pooled);
            return pooled;
        }

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
