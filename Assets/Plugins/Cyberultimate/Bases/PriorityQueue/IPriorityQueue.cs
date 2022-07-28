#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberultimate
{
    public interface IPriorityQueue<T>:IEnumerable<T>,ICollection,ICollection<T>,ICloneable
    {
        T Dequeue();
        void Enqueue(T element);
        T Peek();
    }
   
}
