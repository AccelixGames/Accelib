using System;
using UnityEngine;

namespace Accelib.Module.Deck.Base.View
{
    [System.Serializable]
    public class CardBaseRoot : MonoBehaviour, IEquatable<CardBaseRoot>
    {
        public bool Equals(CardBaseRoot other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other);
        }
    }
}
