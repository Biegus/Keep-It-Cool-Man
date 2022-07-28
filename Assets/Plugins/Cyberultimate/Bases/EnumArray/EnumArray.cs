#nullable enable
using System;
using System.Globalization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
namespace Cyberultimate
{
    public class EnumMap<TValue,TKey>: IDictionary<TKey,TValue>, IReadOnlyDictionary<TKey,TValue>, IStructuralEquatable, IStructuralComparable
        where TKey: Enum
    {
        public ref TValue this[TKey key] => ref array[Convert.ToInt32(key)- start];
        public (int start, int end) Range => (start, start + array.Length - 1);
        
        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] => this[key];
        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get => this[key];
            set => this[key] = value;
        }
        int ICollection<KeyValuePair<TKey, TValue>>.Count => array.Length;
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;
        int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count => array.Length;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Enumerable.Range(start, array.Length).Select(ToEnum);
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => new ReadOnlyCollection<TValue>(this.array);
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => throw new NotSupportedException();
        ICollection<TValue> IDictionary<TKey, TValue>.Values => throw new NotSupportedException();
        
        private readonly TValue[] array;
        private readonly int start;
        private readonly CultureInfo culture = CultureInfo.InvariantCulture;
        
        /// <summary>
        /// End is inclusive
        /// </summary>
        public EnumMap(TKey start, TKey end)
        {
            this.start = Convert.ToInt32(start);
            var iEnd= Convert.ToInt32(end);
            array = new TValue[iEnd - this.start + 1];
        }
        public bool IsInRange(TKey key)
        {
            int val =  Convert.ToInt32(key);
            return val >= start && val < start + array.Length;
        }
       
        public bool ContainsKey(TKey key)
        {
            throw new NotImplementedException();
        }
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (!IsInRange(key))
            {
                value = default!;
                return false;
            }
            value = this[key];
            return true;
        }
        private static TKey ToEnum(int index)
        {
            return (TKey)Enum.ToObject(typeof(TKey), index);
        }
        
        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            if (!(other is EnumMap<TValue,TKey> otherA))
                return false;
            return (array as IStructuralEquatable).Equals(other, comparer) && Range == otherA.Range;
        }
        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            return (array as IStructuralEquatable).GetHashCode(comparer);
        }

        int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            if (!(other is EnumMap<TValue,TKey>))
                return 0;
            return (array as IStructuralComparable).CompareTo(other, comparer);
        }
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return array.Select((element, index) => new KeyValuePair<TKey, TValue>(ToEnum(index), element))
                .GetEnumerator();
        }
      
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
           
            if (IsInRange(item.Key))
                this[item.Key] = item.Value;
            else
                throw new ArgumentException("Key out of range");
        }
        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return IsInRange(item.Key) && object.Equals(this[item.Key],item.Value);
        }
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] ar, int arrayIndex)
        {
            if (ar == null) throw new ArgumentNullException(nameof(ar));
            var enumerator = GetEnumerator();
        
            for (int i = arrayIndex; i < ar.Length&& enumerator.MoveNext(); i++)
                ar[i] = enumerator.Current;
            enumerator.Dispose();

        }
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }
      
        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }
        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
             ((ICollection<KeyValuePair<TKey, TValue>>)this).Add(new KeyValuePair<TKey, TValue>(key, value));
        }
        
    }
}