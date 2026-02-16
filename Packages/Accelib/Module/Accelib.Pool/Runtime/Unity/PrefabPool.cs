using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Accelib.Pool
{
    /// <summary>
    /// MonoBehaviour 프리팹 전용 풀. ComponentPool을 확장하며, Transform 자식 자동 탐색을 지원한다.
    /// </summary>
    [Serializable]
    public class PrefabPool<T> : ComponentPool<T> where T : MonoBehaviour
    {
        [ShowInInspector] private List<T> _enabledList  = new();

        [TitleGroup("# 옵션", indent:true)]
        [SerializeField] private Transform parent;
        [TitleGroup("# 옵션", indent:true)]
        [SerializeField] private T prefab;

        public IReadOnlyList<T> EnabledList => _enabledList;
        public Transform Parent => parent;
        public T Prefab => prefab;

        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;

        public void Initialize(Action<T> onPooled = null, Action<T> onReleased = null)
        {
            _enabledList = new List<T>();
            _releasedList = new List<T>();

            New = () => Object.Instantiate(prefab, parent);
            OnPooled = onPooled ?? (x =>
            {
                x.gameObject.SetActive(true);
                x.transform.SetAsFirstSibling();
            });
            OnReleased = onReleased ?? (x => x.gameObject.SetActive(false));

            foreach (var comp in parent.GetComponentsInChildren<T>())
                base.Release(comp);
            _isInitialized = true;
        }

        public new T Get()
        {
            if (!_isInitialized) throw new InvalidOperationException(
                "PrefabPool has not been initialized. Call Initialize() first.");

            var comp = base.Get();
            _enabledList.Add(comp);
            return comp;
        }

        public new void Release(T comp)
        {
            if (!_enabledList.Remove(comp)) return;

            base.Release(comp);
        }

        public void ReleaseAll()
        {
            if (_enabledList == null) return;

            foreach (var behaviour in _enabledList)
                if(behaviour != null)
                    base.Release(behaviour);
            _enabledList.Clear();
        }
    }
}
