#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberultimate
{
    public interface IReadonlySet<T> : IReadOnlyCollection<T>, IEnumerable<T>
    {
        bool Contains(T item);
    }
}
