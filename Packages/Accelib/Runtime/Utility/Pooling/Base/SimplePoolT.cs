using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Utility.Pooling.Base
{
    public abstract class SimplePoolT<T> : MonoBehaviour where T : Component
    {
        [SerializeField] private List<T> objects;
        
        private void Reset()
        {
            objects = new List<T>();
            foreach (Transform t in transform) objects.Add(t.GetComponent<T>());
        }

        public T GetDisabled() => objects.Find(x => !x.gameObject.activeSelf) ?? objects[0];
    }
}