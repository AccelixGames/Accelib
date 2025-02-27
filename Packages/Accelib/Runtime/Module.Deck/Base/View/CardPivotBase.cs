using Accelib.Module.Deck.Base.Model;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.Deck.Base.View
{
    public class CardPivotBase<SO_DeckDataBase>: MonoBehaviour
    {
        [SerializeField, ReadOnly] private DeckDataInstanceBase<SO_DeckDataBase> dataInstance;
        
        public DeckDataInstanceBase<SO_DeckDataBase> DataInstance => dataInstance;

        public virtual void Clear()
        {
            dataInstance = null;
            transform.localPosition = Vector3.zero;
        }
        
        public virtual void SetData(DeckDataInstanceBase<SO_DeckDataBase> instanceBase)
        {
            dataInstance = instanceBase;
        }
    }
}