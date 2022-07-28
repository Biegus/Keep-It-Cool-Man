#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberultimate
{
    public sealed class MaxPriorityQueue<T> : BasePriorityQueue<T,MaxPriorityQueue<T>>
        where T : IComparable<T>
    {
        protected override bool IsBigger(T parent, T child)
        {
            return parent.CompareTo(child) != -1;
        }
    }
    public sealed class MinPriorityQueue<T>: BasePriorityQueue<T, MinPriorityQueue<T>>
          where T : IComparable<T>
    {
        protected override bool IsBigger(T parent, T child)
        {
            return parent.CompareTo(child) != 1;
        }
    }

}
