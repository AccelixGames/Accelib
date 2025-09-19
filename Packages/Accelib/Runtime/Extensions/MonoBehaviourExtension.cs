using System.Collections.Generic;
using UnityEngine;

namespace Accelib.Extensions
{
    public static class MonoBehaviourExtension
    {
        public static void FindComponents<T>(this Object monoBehaviour, ref List<T> list) where T : class
        {
            if(list == null) list = new List<T>();
            else list.Clear();
            
            foreach (var o in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if(o.TryGetComponent<T>(out var comp))
                    list.Add(comp);
            }
        }
    }
}