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
        [SerializeField] private Transform parent;
        [SerializeField] private T prefab;

        [TitleGroup("디버깅")]
        [ShowInInspector, ReadOnly] private List<T> _enabledList  = new();

        public IReadOnlyList<T> EnabledList => _enabledList;
        public Transform Parent => parent;
        public T Prefab => prefab;

        private bool _isInitialized = false;
        [TitleGroup("디버깅")]
        [ShowInInspector, ReadOnly]
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// 풀을 초기화한다. 부모 Transform의 기존 자식을 자동으로 풀에 등록한다.
        /// </summary>
        /// <param name="onPooled">풀에서 꺼낼 때 콜백. null이면 기본값(SetActive + SetAsFirstSibling) 사용.</param>
        /// <param name="onReleased">풀에 반환할 때 콜백. null이면 기본값(SetActive(false)) 사용.</param>
        /// <exception cref="InvalidOperationException">prefab 또는 parent가 설정되지 않은 경우.</exception>
        public void Initialize(Action<T> onPooled = null, Action<T> onReleased = null)
        {
            // prefab/parent null 방어
            if (prefab == null)
                throw new InvalidOperationException(
                    $"PrefabPool<{typeof(T).Name}>: prefab이 설정되지 않았습니다.");
            if (parent == null)
                throw new InvalidOperationException(
                    $"PrefabPool<{typeof(T).Name}>: parent가 설정되지 않았습니다.");

            // 리스트 초기화
            _enabledList = new List<T>();
            _releasedList = new List<T>();

            // 팩토리 및 콜백 설정
            New = () => Object.Instantiate(prefab, parent);
            OnPooled = onPooled ?? (x =>
            {
                x.gameObject.SetActive(true);
                x.transform.SetAsFirstSibling();
            });
            OnReleased = onReleased ?? (x => x.gameObject.SetActive(false));

            // 부모 아래 기존 자식을 풀에 반환 상태로 등록
            foreach (var comp in parent.GetComponentsInChildren<T>())
                base.Release(comp);
            _isInitialized = true;
        }

        /// <summary>풀에서 인스턴스를 꺼내고 활성 목록에 추가한다.</summary>
        /// <exception cref="InvalidOperationException">Initialize가 호출되지 않은 경우.</exception>
        public new T Get()
        {
            if (!_isInitialized) throw new InvalidOperationException(
                "PrefabPool has not been initialized. Call Initialize() first.");

            var comp = base.Get();
            _enabledList.Add(comp);
            return comp;
        }

        /// <summary>인스턴스를 풀에 반환한다. 활성 목록에 없는 객체는 무시된다.</summary>
        public new void Release(T comp)
        {
            // 활성 목록에서 제거 실패 시 무시
            if (!_enabledList.Remove(comp)) return;

            base.Release(comp);
        }

        /// <summary>모든 활성 인스턴스를 풀에 반환한다.</summary>
        public void ReleaseAll()
        {
            if (_enabledList == null) return;

            // 활성 목록의 모든 인스턴스를 풀에 반환
            foreach (var behaviour in _enabledList)
                if(behaviour != null)
                    base.Release(behaviour);
            _enabledList.Clear();
        }
    }
}
