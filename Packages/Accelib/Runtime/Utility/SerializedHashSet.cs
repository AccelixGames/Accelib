using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Utility
{
    [System.Serializable]
    public class SerializedHashSet<T> : ISerializationCallbackReceiver
    {
        [SerializeField] private List<T> serializedItems;
        private HashSet<T> _hashSet = new();
        
        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            throw new System.NotImplementedException();
        }
    }
}