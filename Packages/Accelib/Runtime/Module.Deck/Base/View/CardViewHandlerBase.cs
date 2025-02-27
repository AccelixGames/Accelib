using System.Collections.Generic;
using Accelib.Extensions;
using Accelib.Module.Deck.Base.Control;
using Accelib.Module.Deck.Base.Model;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Accelib.Module.Deck.Base.View
{
    public abstract class CardViewHandlerBase<T, SO_DeckDataBase> : MonoBehaviour where T : DeckDataInstanceBase<SO_DeckDataBase>, new()
    {
        [Header("Controller")] 
        [SerializeField] protected DeckControllerBase<T, SO_DeckDataBase> deckController;
        
        [Header("Pivot")]
        [SerializeField] protected Transform pivotParent;
        [SerializeField] protected float pivotRange = 10f;
        [SerializeField] protected float maxSpacing = 2f;
        [SerializeField, ReadOnly] protected float spacing = 1f;
        [SerializeField, ReadOnly] protected List<CardPivotBase<SO_DeckDataBase>> pivots;
        
        [Header("Card")]
        [SerializeField] protected Transform cardParent;
        [SerializeField, ReadOnly] protected CardBaseRoot selectedCard;
        [SerializeField, ReadOnly] protected List<CardBase<SO_DeckDataBase>> cards;
        
        [Header("Event")]
        [SerializeField] private CardBaseRootEvent onCardPointerDown;
        [SerializeField] private CardBaseRootEvent onCardPointerUp;
        [SerializeField] private CardBaseRootEvent onCardPointerDrag;

        protected virtual void Awake()
        {
            deckController.onInitEvent.AddListener(Init);
            deckController.onAddEvent.AddListener(OnAdd);
            deckController.onRemoveEvent.AddListener(OnRemove);
            deckController.onEditDataEvent.AddListener(OnEditData);
            deckController.onUpdateEvent.AddListener(OnUpdate);
            
            onCardPointerDown.Register(OnSelectedCard);
            onCardPointerUp.Register(OnSelectedCard);
            onCardPointerDrag.Register(OnDragCard);
        }

        protected virtual void OnDestroy()
        {
            deckController.onAddEvent.RemoveListener(OnAdd);
            deckController.onRemoveEvent.RemoveListener(OnRemove);
            deckController.onEditDataEvent.RemoveListener(OnEditData);
            deckController.onUpdateEvent.RemoveListener(OnUpdate);
            
            onCardPointerDown.Unregister(OnSelectedCard);
            onCardPointerUp.Unregister(OnSelectedCard);
            onCardPointerDrag.Unregister(OnDragCard);
        }


        protected virtual void Init()
        {
            pivots = new List<CardPivotBase<SO_DeckDataBase>>();
            cards = new List<CardBase<SO_DeckDataBase>>();
        }
        

        protected virtual void OnAdd(T instance)
        {
            var pivot = GetNewPivot();
            if (pivot != null)
            {
                pivot.SetData(instance);
                pivots.Add(pivot);
            }
            
            var card = GetNewCard();
            if (card != null)
            {
                cards.Add(card);
                card.SetData(instance);
            }
        }

        protected virtual void OnRemove(T instance)
        {
            var pivot = pivots.Find(x => x.DataInstance.Equals(instance));
            if (pivot != null)
            {
                pivots.Remove(pivot);
                pivot.Clear();
            }
            
            var card = cards.Find(x => x.DataInstance.Equals(instance));
            if (card != null)
            {
                cards.Remove(card);
                card.Clear();
            }
        }

        protected virtual void OnEditData(T instance)
        {
            var card = cards.Find(x => x.DataInstance.Equals(instance));
            if (card != null)
            {
                card.Load();
            }
        }

        protected virtual void OnUpdate()
        {
            // 피봇 위치 갱신
            UpdatePivots();

            // 카드 정보 갱신
            UpdateCards();
            
        }

        /// <summary>
        /// deckController의 데이터 대로 피봇 위치 업데이트
        /// </summary>
        protected virtual void UpdatePivots()
        {
            // 현재 덱 크기
            var count = deckController.HandDeck.Count;
            
            // 카드 간격
            spacing = Mathf.Clamp(pivotRange / count, 0, maxSpacing);
            
            // 덱 크기에 따른 시작 위치 설정
            var halfCount = count / 2;
            var isEven = count % 2 == 0;
            var firstPos = (isEven ? halfCount - 0.5f : halfCount) * spacing;

            
            // 제거할 피봇
            var removePivots = new List<CardPivotBase<SO_DeckDataBase>>();
            
            // 피봇 순회
            foreach (var pivot in pivots)
            {
                // 덱에 있는 데이터와 같은 피봇의 위치 변경
                var idx = deckController.HandDeck.FindIndex(x => x == pivot.DataInstance);
                if (idx >= 0)
                {
                    var offset = new Vector3(idx * spacing - firstPos, 0, -0.001f * idx);
                    pivot.transform.position = pivotParent.position + offset;
                }
                else
                {
                    removePivots.Add(pivot);
                }
            }

            // 덱에 없는 데이터를 가진 피봇 제거
            foreach (var pivot in removePivots)
            {
                pivots.Remove(pivot);
                pivot.Clear();
            }
        }

        protected virtual void UpdateCards()
        {
            var removeCards = new List<CardBase<SO_DeckDataBase>>();
            foreach (var card in cards)
            {
                // 피봇과 같은 데이터 객체를 가지고 있다면 이동
                var pivot = pivots.Find(x => x.DataInstance.Equals(card.DataInstance));
                if (pivot)
                {
                    card.MoveTo(pivot.transform.position);
                    card.Activate(true);
                }
                // 아니라면 제거
                else
                {
                    removeCards.Add(card);
                }
            }
            
            foreach (var card in removeCards)
            {
                cards.Remove(card);
                card.Clear();
            }
        }

        protected virtual void OnSelectedCard(CardBaseRoot cardBase)
        {
            
            Debug.Log("OnSelectedCard Call!!");

            selectedCard = cardBase;
            OnUpdate();
        }

        protected virtual void OnDragCard(CardBaseRoot cardBase){ Debug.Log("OnDragCard Call!! : Base"); }
        protected abstract CardPivotBase<SO_DeckDataBase> GetNewPivot();
        protected abstract CardBase<SO_DeckDataBase> GetNewCard();

    }
}