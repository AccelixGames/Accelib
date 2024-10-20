using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Accelib.Extensions
{
    public static class ReadonlyListExtenstion
    {
        public static T Find<T>(this IReadOnlyList<T> list, [NotNull] Predicate<T> predicate)
        {
            return list.ToList().Find(predicate);
        }
        
        public static int FindIndex<T>(this IReadOnlyList<T> list, [NotNull] Predicate<T> match)
        {
            return list.ToList().FindIndex(match);
        }
        
        public static int IndexOf<T>(this IReadOnlyList<T> list , T item)
        {
            return list.ToList().IndexOf(item);
        }
    }
}