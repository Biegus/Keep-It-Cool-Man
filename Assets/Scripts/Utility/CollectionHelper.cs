using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace LetterBattle.Utility
{
    
    public static class CollectionHelper
    {

        public static string ToStringFromCollection<T>(this IEnumerable<T> collection, string separator = ", " )
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));
            return collection.Aggregate(new StringBuilder(), (b, v) => b.Append($"{v}{separator}")).ToString();
        }
        public static void Foreach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection is null) throw new ArgumentNullException();
            if (action is IReadOnlyList<T> list)
            {
                //faster
                for (int i = 0; i < list.Count; i++)
                {
                    action(list[i]);
                }
            }
            foreach (var element in collection)
            {
                action(element);
            }
        }
        public static string BuildString<T>(this IEnumerable<T> collection, Func<T,int,string> getter=null, bool skipLast=false)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            
            getter ??= (v,i) => v.ToString();
            int index = 0;
            var builder = collection.Aggregate(new StringBuilder(), (b, v) => b.Append(getter(v, index++)));
            if (skipLast && builder.Length>0)
                builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }
        public static T1 GetRandomWeightItemFromValue<T1>(this IList<T1> collection, Func<T1, float> weightGetter
        , float random,T1 emptyValue=default)
        {
            if (collection.Count == 0)
                return emptyValue;
            float sum = 0;
            (float a, float b)[] ranges = new (float a, float b)[collection.Count+1];
            ranges[0] = (0,0);
          
            for (var i = 0; i < collection.Count; i++)
            {
                var item = collection[i];
                var weight = weightGetter(item);
                ranges[i+1] = (ranges[i].b, ranges[i].b + weight);
                sum += weight;
            }
            float movedRandom = random * sum;
            for (int i = 1; i < ranges.Length; i++)
            {
                if (movedRandom >= ranges[i].a && movedRandom <= ranges[i].b)
                    return collection[i - 1];
            }
            throw new InvalidOperationException("GetRandomWeightItemFromValue algorithm failed somehow");
        }
    }
}