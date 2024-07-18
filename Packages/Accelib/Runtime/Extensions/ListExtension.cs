using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Accelib.Extensions
{
    public static class ListExtension
    {
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