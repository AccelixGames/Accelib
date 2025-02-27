using Accelib.Module.Deck.Base.Model;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Accelib.Module.Deck.Base.View
{
    public class CardBase<SO_DeckDataBase>: CardBaseRoot, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField, ReadOnly] private DeckDataInstanceBase<SO_DeckDataBase> dataInstance;
        public DeckDataInstanceBase<SO_DeckDataBase> DataInstance => dataInstance;

        [Header("Card Events")]
        [SerializeField] protected CardBaseRootEvent onCardPointerDown;
        [SerializeField] protected CardBaseRootEvent onCardPointerUp;
        [SerializeField] protected CardBaseRootEvent onCardPointerDrag;
        
        [Header("Drag")]
        [SerializeField] protected float offsetZ = -0.5f;
        
        protected virtual bool IsDragging { get; set; }

        
        public virtual void Clear()
        {
            dataInstance = null;
        }
        
        public virtual void SetData(DeckDataInstanceBase<SO_DeckDataBase> instanceBase)
        {
            dataInstance = instanceBase;
        }

        public virtual void Load()
        {
            if (dataInstance == null) return;
            
        }

        public virtual void MoveTo(Vector3 position)
        {
            transform.position = position;
        }

        public virtual void Activate(bool value)
        {
            gameObject.SetActive(value);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            IsDragging = true;
            onCardPointerDown?.Raise(this);
        }
        
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            IsDragging = false;
            onCardPointerUp?.Raise(null);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!IsDragging) return;
            if (!Camera.main) return;
            
            // 포인터 위치로 이동
            var worldPosition= Camera.main.ScreenToWorldPoint(eventData.position);
            worldPosition.z = offsetZ;
            
            MoveTo(worldPosition);
  
            onCardPointerDrag?.Raise(this);
        }
    }
}