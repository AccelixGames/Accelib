using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Accelib.Module.Reference
{
    [CreateAssetMenu(fileName = "(Reference) Default", menuName = "Accelib/Reference", order = 0)]
    public class SOReference : ScriptableObject
    {
        [SerializeField, ReadOnly] private List<GameObject> references;

        public List<T> FindByType<T>() where T : class
        {
            var list = new List<T>();
            foreach (var refer in references)
                if (refer.TryGetComponent<T>(out var comp))
                    list.Add(comp);

            return list;
        }
        
        public void Register(GameObject gameObject)
        {
            if(!references.Contains(gameObject))
                references.Add(gameObject);
        }

        public void UnRegister(GameObject gameObject)
        {
            references.Remove(gameObject);
        }
    }
}