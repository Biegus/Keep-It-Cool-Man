#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyberultimate.Unity
{
    /// <summary>
    /// Equalizer for collection returns true for sequentially identical collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SequenceEqualityComparer<T> :  IEqualityComparer<IEnumerable<T>>
    {
        /// <summary>
        /// The same as <see cref="Enumerable.SequenceEqual{TSource}(IEnumerable{TSource}, IEnumerable{TSource})"/>
        /// </summary>
        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            if (x == y)
                return true;
            else if (x == null || y == null)
                return false;
            return x.SequenceEqual(y);
        }

        /// <summary>
        /// Returns a hash code identical to sequentially identical collections
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(IEnumerable<T> obj)
        {
            int hashCode = 373119288;
            foreach(var item in obj)
            {
                hashCode = hashCode * -1521134295 + item.GetHashCode();
            }
            return hashCode;
        }
    }
    
}
