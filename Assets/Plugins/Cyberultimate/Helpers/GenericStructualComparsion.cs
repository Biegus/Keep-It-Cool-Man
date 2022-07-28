#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberultimate
{
    /// <summary>
    /// Generic version for the <see cref="StructuralComparisons"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericStructualComparsion<T>
    {

        public static IComparer<T> StructualComparer { get; } = new StructalComparer();
        public static IEqualityComparer<T> StructualEqualityComparer { get; } = new StructalEqualityComparer();
        private class StructalComparer : IComparer<T>
        {
            public int Compare(T x, T y)
            {
                return StructuralComparisons.StructuralComparer.Compare(x, y);
            }
        }
        private class StructalEqualityComparer : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
            }

            public int GetHashCode(T obj)
            {
                return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
            }
        }
    }
}

