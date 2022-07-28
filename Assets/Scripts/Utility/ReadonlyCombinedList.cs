using System.Collections;
using System.Collections.Generic;

namespace LetterBattle.Utility
{
    public class ReadonlyCombinedList<T> : IReadOnlyList<T>
    {
        private IReadOnlyList<T> a, b;
        public ReadonlyCombinedList(IReadOnlyList<T> a, IReadOnlyList<T> b)
        {
            this.a = a;
            this.b = b;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var value in a)
                yield return value;
            foreach (T value in b)
                yield return value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => a.Count + b.Count;

        public T this[int index]
        {
            get
            {
                if (index >= a.Count)
                    return b[index - a.Count];
                else return a[index];
            }
        }
    }
}