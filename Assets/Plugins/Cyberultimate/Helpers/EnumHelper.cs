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
    /// <summary>
    /// Includes missing enum functions.
    /// </summary>
    public static class EnumExtension
    {

        /// <summary>
        /// Generic version for <see cref="System.Enum.GetValues(Type)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetValues<T>()
            where T : Enum, IConvertible
        {
            return Enum.GetValues(typeof(T)).OfType<T>();
        }

    }
}
