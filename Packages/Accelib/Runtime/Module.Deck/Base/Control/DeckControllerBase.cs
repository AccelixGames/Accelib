using System.Collections.Generic;
using Accelib.Logging;
using Accelib.Module.Deck.Base.Model;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Accelib.Module.Deck.Base.Control
{
    public class DeckControllerBase<T, SO_DeckDataBase> : MonoBehaviour where T : DeckDataInstanceBase<SO_DeckDataBase>, new()
    {
        [SerializeField, ReadOnly] protected List<T> handDeck;
        public IReadOnlyList<T> HandDeck => handDeck;
        
        [SerializeField] private bool isDisplayEvent;
        
        [ShowIf(nameof(isDisplayEvent))] public UnityEvent onInitEvent = new();
        [ShowIf(nameof(isDisplayEvent))] public UnityEvent<T> onAddEvent = new();
        [ShowIf(nameof(isDisplayEvent))] public UnityEvent<T> onRemoveEvent = new();
        [ShowIf(nameof(isDisplayEvent))] public UnityEvent<T> onEditDataEvent = new();
        [ShowIf(nameof(isDisplayEvent))] public UnityEvent onUpdateEvent = new();

        
        public virtual void Awake()
        {
            Init();
        }
        
        /// <summary>
        /// 덱 초기화
        /// </summary>
        public virtual void Init()
        {
            handDeck = new List<T>();
        }
        
        /// <summary>
        /// 덱에 데이터 객체 추가
        /// </summary>
        /// <param name="data"></param>
        public virtual void OnAdd(SO_DeckDataBase data)
        {
            handDeck ??= new List<T>();

            var instance = new T();
            instance.Init(data);

            if (handDeck.Contains(instance))
            {
                Deb.LogWarning($"{instance} 데이터 추가 실패 : 기존에 있는 객체입니다.");
                return;

            } 
            handDeck.Add(instance);
            onAddEvent?.Invoke(instance);

            OnUpdate();
        }

        /// <summary>
        /// 덱에서 데이터 객체 제거
        /// </summary>
        /// <param name="instance">대상 객체</param>
        public virtual void OnRemove(T instance)
        {
            if (!handDeck.Contains(instance))
            {
                Deb.LogWarning($"{instance} 데이터 제거 실패 : 덱에 없는 객체입니다.");
                return;
            }
            
            handDeck.Remove(instance);
            onRemoveEvent?.Invoke(instance);

            OnUpdate();
        }

        /// <summary>
        /// 덱에 있는 객체의 data를 새로운 data로 갱신
        /// </summary>
        /// <param name="instance">대상 객체</param>
        /// <param name="newData">변경하려는 데이터</param>
        public virtual void OnEditData(T instance, SO_DeckDataBase newData)
        {
            var ins = handDeck.Find(x => x.Equals(instance));
            if (ins == null)
            {
                Deb.LogWarning($"{instance}의 데이터 갱신 실패 : instance를 찾을 수 없습니다.");
                return;
            }

            ins.Init(newData);
            onEditDataEvent?.Invoke(ins);

            OnUpdate();
        }

        /// <summary>
        /// 덱에 있는 객체의 순서fmf index 위치로 변경
        /// </summary>
        /// <param name="instance">대상 객체</param>
        /// <param name="index">변경하려는 위치</param>
        public virtual void OnInsert(T instance, int index)
        {
            if (handDeck == null || index >= handDeck.Count || !handDeck.Contains(instance))
            {
                Deb.LogWarning($"{instance}의 순서 변경 실패");
                return;
            }
            
            handDeck.Remove(instance);
            handDeck.Insert(index, instance);

            OnUpdate();
        }

        /// <summary>
        /// index 위치의 요소와 대상 객체 위치의 요소를 교환
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="index"></param>
        public virtual void OnSwap(T instance, int index)
        {
            var targetIndex = handDeck.FindIndex(x => x.Equals(instance));

            if (targetIndex <= -1)
            {
                Deb.LogWarning($"{index}번째에 해당하는 객체를 찾을 수 없습니다.");
                return;
            }
            
            // swap
            (handDeck[index], handDeck[targetIndex]) = (handDeck[targetIndex], handDeck[index]);
            
            OnUpdate();
        }

        /// <summary>
        /// 덱 목록을 최신 상태로 업데이트
        /// </summary>
        public virtual void OnUpdate()
        {
            onUpdateEvent?.Invoke();
        }
    }
}