using System.Collections.Generic;
using System.Linq;
using Accelib.Logging;
using UnityEngine;

namespace Accelib.Extensions
{
    public static class ListExtension
    {
        public static T GetOrDefault<T>(this List<T> list, int index, T defaultValue = default)
        {
            if (list != null && index < list.Count)
                return list[index];
            
            return defaultValue;
        }
        
        public static T GetOrDefault<T>(this IEnumerable<T> list, int index, T defaultValue = default)
        {
            if (list != null && index < list.Count())
                return list.ElementAt(index);
            
            return defaultValue;
        }
        
        public static void AddRangeNullCheck<T>(this List<T> list, IEnumerable<T> collection)
        {
            if(collection == null) return;

            list.AddRange(collection);
        }
        
        public static List<T> Shuffle<T>(this List<T> list)
        {
            for (var i = 0; i < list.Count; ++i)
            {
                var random1 = UnityEngine.Random.Range(0, list.Count);
                var random2 = UnityEngine.Random.Range(0, list.Count);

                (list[random1], list[random2]) = (list[random2], list[random1]);
            }

            return list;
        }
        
        public static List<T> GetRandomElements<T>(this List<T> list, int n)
        {
            if (list == null) return null;
            if (n < 0) return null;
            n = Mathf.Min(list.Count, n);

            return list.OrderBy(x => UnityEngine.Random.value).Take(n).ToList();
        }
        
        public static IEnumerable<T> GetRandomElements<T>(this IEnumerable<T> list, int n)
        {
            if (list == null) return null;
            if (n < 0) return null;
            n = Mathf.Min(list.Count(), n);

            return list.OrderBy(x => UnityEngine.Random.value).Take(n).ToList();
        }

        public static T GetRandom<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null ? default : enumerable.ToList().GetRandom();
        }

        public static T GetRandom<T>(this List<T> list)
        {
            if (list is not { Count: > 0 }) return default;
            
            var id = UnityEngine.Random.Range(0, list.Count);
            return list[id];
        }
    }
}