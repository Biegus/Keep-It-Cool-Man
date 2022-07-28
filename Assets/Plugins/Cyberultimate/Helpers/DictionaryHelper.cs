#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cyberultimate
{
    public static class DictioniaryExtension
    {
        public static void AddRange<TKey,TValue>(this IDictionary<TKey,TValue> dictionary, IEnumerable<KeyValuePair<TKey,TValue>> values)
        {
            if (values is null)
                throw new ArgumentNullException(nameof(values));
            foreach(var item in values)
            {
                dictionary[item.Key] = item.Value;
            }
        }
        public static T2 GetOrAdd<T1, T2>(this IDictionary<T1, T2> dictioniary, T1 key, T2 ifEmpty)
        {
            if (dictioniary is null)
            {
                throw new ArgumentNullException(nameof(dictioniary));
            }
            if (dictioniary.TryGetValue(key, out T2 val))
            {
                return val;
            }
            else
            {
                dictioniary[key] = ifEmpty;
                return ifEmpty;
            }

        }
        public static Dictionary<TKey, IGrouping<TKey, TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> groups)
        {
            return groups.ToDictionary(item => item.Key, item => item);
        }
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this Dictionary<TKey, TValue> dictioniary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictioniary);
        }
    }
   
}
