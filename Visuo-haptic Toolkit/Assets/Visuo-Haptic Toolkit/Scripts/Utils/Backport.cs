using System;
using System.Collections.Generic;
using System.Linq;

namespace VHToolkit {
    /// <summary>
    /// Provide language features not present in this version of .Net.
    /// </summary>
    static class Future {
        public static IEnumerable<(TFirst First, TSecond Second)> Zip<TFirst,TSecond> (
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second
        ) => first.Zip(second, resultSelector: (x, y) => (x, y));

        // TODO fix behaviour with nullable types
        public static TSource MaxBy<TSource,TKey> (this IEnumerable<TSource> source, Func<TSource,TKey> keySelector) where TKey: IComparable<TKey> {
            TSource result = default;
            TKey value =  default;
            foreach(var x in source) {
                var newValue = keySelector(x);
                if (result is null || newValue.CompareTo(value) > 0) {
                    result = x;
                    value = newValue;
                }
            }
            return result;
        }

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : IComparable<TKey>
        {
            if (source is null) throw new ArgumentNullException();
            TSource result = default;
            TKey value = default;
            foreach (var x in source)
            {
                var newValue = keySelector(x);
                if (result is null || newValue.CompareTo(value) < 0)
                {
                    result = x;
                    value = newValue;
                }
            }
            return result;
        }
    }

}
