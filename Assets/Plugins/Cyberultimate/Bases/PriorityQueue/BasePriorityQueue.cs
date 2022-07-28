#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections;

namespace Cyberultimate
{

    public abstract class BasePriorityQueue<T,TSelf> : IPriorityQueue<T>
        where T : IComparable<T>
        where TSelf: BasePriorityQueue<T, TSelf>,new()
    {
        public ReadOnlyCollection<T> Cheap { get; private  set; }
        public int Count => cheap.Count;
    
        private readonly List<T> cheap = new List<T>();
    

        protected BasePriorityQueue()
        {
            Cheap = new ReadOnlyCollection<T>(cheap);
        }

        public void Enqueue(T element)
        {
            cheap.Add(element);
            Swim(cheap.Count - 1);
        }
        public T Dequeue()
        {
            T element = cheap[0];
            Swap(0, cheap.Count - 1);
            cheap.RemoveAt(cheap.Count - 1);
            Sink(0);
            return element;
        }
        public T Peek()
        {
            return cheap[0];
        }
        public void Clear()
        {
            cheap.Clear();
        }
        public bool Remove(T item)
        {
            int index = cheap.IndexOf(item);
            if (index == -1)
                return false;
            RemoveAt(index);
            return true;
        }
        public bool Contains(T item)
        {
            return cheap.Contains(item);
        }
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= cheap.Count)
                throw new ArgumentException("Wrong index");
            Swap(index, cheap.Count - 1);
            cheap.RemoveAt(cheap.Count - 1);
            Sink(index);
            
        }
     
        public TSelf Clone()
        {
            TSelf self = new TSelf();
            self.cheap.AddRange(this.cheap);
            return self;
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            (this as ICollection).CopyTo(array, arrayIndex);
        }
      
        private void Swap(int a, int b)
        {
            T temp = cheap[a];
            cheap[a] = cheap[b];
            cheap[b] = temp;
        }
        private void Swim(int index)
        {
            while (true)
            {
                int parent = (index - 1) / 2;
                if (parent < 0 || IsBigger( cheap[parent],cheap[index]))
                    break;
                Swap(parent, index);
                index = parent;

            }
        }
        private void Sink(int index)
        {
            while (true)
            {
                int childA = (index * 2) + 1;
                int childB = (index * 2) + 2;
                int maxChild = -2;//-2 not inited, -1 neither of child is correct
                if (childA >= cheap.Count)
                {
                    childA = -1;
                    maxChild = childB;
                }
                  
                if (childB >= cheap.Count)
                {
                    childB = -1;
                    maxChild = childA;
                }
                if (maxChild == -2)
                    maxChild = IsBigger( cheap[childA],(cheap[childB])) ? childA : childB;
                if (maxChild == -1|| maxChild >= cheap.Count || IsBigger((cheap[index]),(cheap[maxChild])) )//neither of child exists
                    break;

                Swap(index, maxChild);
                index = maxChild;

            }

        }
        protected abstract bool IsBigger(T parent, T child);
        object ICloneable.Clone()
        {
            return Clone();
        }
        bool ICollection<T>.IsReadOnly => false;
        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => this;

        public IEnumerator<T> GetEnumerator()
        {
            TSelf clone = Clone();
            for(int x=0;x<this.Count;x++)
            {
                yield return clone.Dequeue();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
      

        void ICollection<T>.Add(T item)
        {
            Enqueue(item);
        }
        void ICollection.CopyTo(Array array, int index)
        {

            if (array is null)
                throw new ArgumentNullException(nameof(array));
            if (index < 0)
                throw new ArgumentException("Array index below zero");
            foreach (T item in this)
            {
                array.SetValue(item, index);
;            }
        }
    }
}
