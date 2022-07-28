#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Cyberultimate.Unity
{
    [Serializable]
    public struct SerializedTuple<T1, T2> : IStructuralComparable, IComparable, IComparable<(T1, T2)>, IEquatable<(T1, T2)>
    {
        [SerializeField]
        public T1 X;
        [SerializeField]
        public T2 Y;

        public object this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        

        public ValueTuple<T1, T2> ToValueTuple()
        {
            return (X, Y);
        }
        public int CompareTo((T1, T2) other)
        {
            return (X, Y).CompareTo(other);
        }

        public bool Equals((T1, T2) other)
        {
            return (X, Y).Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = -798337671;
            hashCode = hashCode * -1521134295 + EqualityComparer<T1>.Default.GetHashCode(X);
            hashCode = hashCode * -1521134295 + EqualityComparer<T2>.Default.GetHashCode(Y);
            return hashCode;
        }

        int IStructuralComparable.CompareTo(object other, IComparer comparer)
        {
            return ((X, Y) as IStructuralComparable).CompareTo(other, comparer);
        }

        int IComparable.CompareTo(object obj)
        {
            return ((X, Y) as IComparable).CompareTo(obj);
        }

      
    }
}
