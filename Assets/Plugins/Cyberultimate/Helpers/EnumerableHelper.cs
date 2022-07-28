#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;


namespace Cyberultimate
{
    public static class EnumerableExtension
    {
        /// <summary>
        /// Returns index of element.
        /// </summary>
        public static int? Index<T>(this IEnumerable<T> collection, T element)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            return Index(collection, (el) => System.Object.Equals(element, el));
        }
        /// <summary>
        /// Returns index of predicate.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static int? Index<T>(this IEnumerable<T> collection, Func<T, bool> func)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            int i = 0;
            foreach (T item in collection)
            {

                if (func(item))
                    return i;
                i++;
            }
            return null;

        }
        /// <summary>
        /// Crosses two collection.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T> Cross<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            using IEnumerator<T> enA = a.GetEnumerator();
            using IEnumerator<T> enB = b.GetEnumerator();
            bool aEnd = true;
            bool bEnd = true;
            while (aEnd || bEnd)
            {
              
                if (aEnd = enA.MoveNext())
                    yield return enA.Current;
             
                if (bEnd = enB.MoveNext())
                    yield return enB.Current;
            }
            yield break;
        }
        public static T1 MinOriginal<T1, T2>(this IEnumerable<T1> collection, Func<T1, T2> func)
            where T2 : IComparable
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            return BaseChecker(collection, func, (a, b) => a.CompareTo(b) == 1);
        }
        public static T1 MaxOriginal<T1, T2>(this IEnumerable<T1> collection, Func<T1, T2> func)
              where T2 : IComparable
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            return BaseChecker(collection, func, (a, b) => a.CompareTo(b) == -1);
        }
        private static T1 BaseChecker<T1, T2>(this IEnumerable<T1> collection, Func<T1, T2> func, Func<T2, T2, bool> comparer)
            where T2 : IComparable
        {


            bool isWrong = true;
            T2 lowest = default;
            T1 lowestRepresent = default;
            foreach (T1 item in collection)
            {
                T2 rawVal = func(item);
                if (comparer(lowest, rawVal) || isWrong)
                {
                    isWrong = false;
                    lowest = rawVal;
                    lowestRepresent = item;
                }
            }
            return lowestRepresent!;
        }

        public static object[] ToObjectArray(this IEnumerable enumerable)
        {
            if (enumerable is null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            return enumerable.OfType<object>().ToArray();
        }
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector is null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            HashSet<TKey> knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

    }
}
