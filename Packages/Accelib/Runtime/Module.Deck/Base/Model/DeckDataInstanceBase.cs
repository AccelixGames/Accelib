using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.Deck.Base.Model
{
    [System.Serializable]
    public abstract class DeckDataInstanceBase<SO_DeckDataBase>
    {
        [SerializeField, ReadOnly] private SO_DeckDataBase data;
        
        public SO_DeckDataBase Data => data;

        public void Init(SO_DeckDataBase newData)
        {
            data = newData;

            // todo : 자식에서 구현
            OnInit();
        }

        protected abstract void OnInit();
    }
}