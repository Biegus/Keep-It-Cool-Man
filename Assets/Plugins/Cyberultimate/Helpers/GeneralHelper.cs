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
    public static class GeneralExtension
    {

        private class ScalarStruct<T> : IEnumerable<T>
        {
            public T Value { get; }
            public ScalarStruct(T value)
            {
                this.Value = value;
            }
            public IEnumerator<T> GetEnumerator()
            {
                yield return Value;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }


        }
        /// <summary>
        /// Converts given element into <see cref="IEnumerable{T}"/> interface.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IEnumerable<T> Scalar<T>(this T element)
        {
            return new ScalarStruct<T>(element);
        }
        /// <summary>
        /// Generate more readable version for dictionaries and collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="maxDeepLv"></param>
        /// <returns></returns>
        public static string ToDebugString<T>(this T obj, int maxDeepLv = 10)
        {
            return ToDebugString(obj, 0, maxDeepLv);
        }
        private static string ToDebugString<T>(this T obj, uint deep, int maxDeeplv)
        {

            string basic = obj?.ToString() ?? ">null<";
            if (deep > maxDeeplv)
            {
                return basic;
            }
            uint next = deep + 1;
            if ((basic == (obj?.GetType().ToString() ?? string.Empty)))
            {

                StringBuilder elementBullider = new StringBuilder();
                bool first = true;
                string SaveToDebugString(object val)
                {
                    if (System.Object.ReferenceEquals(val, obj))
                    {
                        return "this";
                    }
                    else
                        return ToDebugString(val, next, maxDeeplv);
                }
                void CheckFirst()
                {
                    if (first == false)
                    {
                        elementBullider.Append(", ");

                    }

                    first = false;
                }
                switch (obj)
                {
                    case IDictionary dict:

                        foreach (var key in dict.Keys)
                        {
                            CheckFirst();
                            elementBullider.Append($"{{{SaveToDebugString(key)}}}={{{SaveToDebugString(dict[key])}}}");
                        }
                        return $"{{ {elementBullider} }}";
                    case IEnumerable collection:


                        foreach (var element in collection)
                        {
                            CheckFirst();
                            elementBullider.Append(SaveToDebugString(element));
                        }
                        return $"{{ {elementBullider} }}";
                }
            }
            return basic;
        }
        public static T As<T>(this object obj)
            where T:class
        {
            return obj as T;
        }
    }
}
