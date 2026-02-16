using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZLinq;
using ZLinq.Linq;

namespace Accelib.Extensions
{
    public static class ListExtension
    {
        public static T Peek<T>(this List<T> list, int index)
        {
            if (list.Count <= 0 || index >= list.Count || index < 0)
                return default;
            
            var element = list[index];
            list.RemoveAt(index);
            return element;
        }
        
        public static T PeekLast<T>(this List<T> list)
        {
            if (list.Count <= 0)
            {
                Debug.LogWarning("List is empty");
                return default;
            }
            
            var idx = list.Count - 1;
            var element = list[idx];
            list.RemoveAt(idx);
            return element;
        }
        
        public static IEnumerable<T> DeepCopy<T>(this IEnumerable<T> list) where T : ICloneable => 
            list.Select(item => (T)item.Clone());

        public static List<T> DeepCopy<T>(this List<T> list) where T : ICloneable => 
            list.Select(item => (T)item.Clone()).ToList();
        
        public static T GetOrDefault<T>(this IEnumerable<T> list, int index, T defaultValue = default)
        {
            if (list != null && index < list.Count() && index >= 0)
                return list.ElementAtOrDefault(index);
            
            return defaultValue;
        }
        
        public static void AddNullCheck<T>(this List<T> list, T item)
        {
            if (item == null) return;
            
            list.Add(item);
        }
        
        public static void AddRangeNullCheck<T>(this List<T> list, IEnumerable<T> collection)
        {
            if(collection == null) return;

            list.AddRange(collection);
        }
        
        public static List<T> Shuffle<T>(this IEnumerable<T> list) => 
            list.AsValueEnumerable().Shuffle().ToList();

        // 리스트를 n개씩 끊어서, 각 범위 안에서만 섞는다.
        public static void ShuffleInChunks<T>(this List<T> list, int chunkSize)
        {
            if (chunkSize <= 0)
                throw new ArgumentException("chunkSize must be greater than 0");

            var count = list.Count;

            for (var i = 0; i < count; i += chunkSize)
            {
                var end = Math.Min(i + chunkSize, count);
                ShuffleRange(list, i, end);
            }
        }

        // 리스트의 부분 범위 [start, end) 를 섞는 내부 함수
        public static void ShuffleRange<T>(this List<T> list, int start, int end)
        {
            var rng = new System.Random();
            for (var i = end - 1; i > start; i--)
            {
                var j = rng.Next(start, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
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
            if(enumerable == null) return default;

            var vEnum = enumerable.AsValueEnumerable();
            var count = vEnum.Count();
            if (count <= 0) return default;
            
            var id = UnityEngine.Random.Range(0, count);
            return vEnum.ElementAtOrDefault(id);
        }

        public static string ToString<T>(this IEnumerable<T> enumerable, string separator = ",") => 
            string.Join(separator, enumerable);
        
        public static string ToString<T>(this List<T> enumerable, string separator = ",") => 
            string.Join(separator, enumerable);
        
        public static bool UnorderedEquals<T>(this IEnumerable<T> a, in IEnumerable<T> b)
        {
            if (a == null || b == null) return false;
            
            return UnorderedEquals(a.AsValueEnumerable(), b.AsValueEnumerable());
        }
        
        private static bool UnorderedEquals<T>(this ValueEnumerable<FromEnumerable<T>, T> compA,
            ValueEnumerable<FromEnumerable<T>, T> compB)
        {
            try
            {
                // 개수가 다르면 종료
                if (compA.Count() != compB.Count())
                    return false;

                var countDict = new Dictionary<T, int>();

                foreach (var s in compA)
                    if (!countDict.TryAdd(s, 1))
                        countDict[s] += 1;

                foreach (var s in compB)
                    if (s != null && countDict.ContainsKey(s))
                        countDict[s]--;
                    else
                        return false;

                return countDict.Values.AsValueEnumerable().All(c => c == 0);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
    }
}