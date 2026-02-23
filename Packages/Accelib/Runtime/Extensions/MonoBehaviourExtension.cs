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

            var visitedBuffer = new HashSet<GameObject>();
            var buffer = new List<T>();

            // 모든 MonoBehaviour를 순회하며 T를 구현하는 컴포넌트 수집
            foreach (var o in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                var go = o.gameObject;
                if (!visitedBuffer.Add(go)) continue;

                go.GetComponents(buffer);
                foreach (var comp in buffer)
                    list.Add(comp);
            }
        }

        public static void FindComponents<T>(this Object monoBehaviour, ref HashSet<T> list) where T : class
        {
            if (list == null) list = new HashSet<T>();
            else list.Clear();

            var visitedBuffer = new HashSet<GameObject>();
            var buffer = new List<T>();

            // 모든 MonoBehaviour를 순회하며 T를 구현하는 컴포넌트 수집
            foreach (var o in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                var go = o.gameObject;
                if (!visitedBuffer.Add(go)) continue;

                go.GetComponents(buffer);
                foreach (var comp in buffer)
                    list.Add(comp);
            }
        }
    }
}