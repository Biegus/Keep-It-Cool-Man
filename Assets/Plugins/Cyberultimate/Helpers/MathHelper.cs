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
    public static class MathHelper
    {
        /// <summary>
        /// Converts the value from the basic range to a value from the needed range.
        /// </summary>
        /// <param name="basic">Basic range.</param>
        /// <param name="needed">Needed range.</param>
        /// <param name="value">Value from the basic range.</param>
        /// <returns></returns>
        public static double ReCalculateRange((double min, double max) basic, (double min, double max) needed, double value)
        {
            return (value - basic.min) / (basic.max - basic.min) * (needed.max - needed.min) + needed.min;
        }
        /// <summary>
        /// Converts the value from basic range to a value from needed range.
        /// </summary>
        /// <param name="basic">Basic range.</param>
        /// <param name="needed">Needed range.</param>
        /// <param name="value">Value from basic range.</param>
        /// <returns></returns>
        public static float ReCalculateRange((float min, float max) basic, (float min, float max) needed, float value)
        {
            return (value - basic.min) / (basic.max - basic.min) * (needed.max - needed.min) + needed.min;
        }
        /// <summary>
        /// Generic methods can be used on all number types, although they are way slower.
        /// </summary>
        public class Generic
        {
            public static T Clamp<T>(T value, T min, T max)
                where T : IComparable<T>
            {

                int maxCompare = value.CompareTo(max);
                if (maxCompare == 1 || maxCompare == 0)
                    return max;
                int minComparer = value.CompareTo(min);
                if (minComparer == 1)
                    return value;
                else return min;

            }
            public static T GeneralCheckedRemove<T>(T a, T b, Func<T, T, T> minusOperation, T min)
                where T : IComparable<T>
            {
                T different = minusOperation(a, min);
                if (different.CompareTo(b) != -1)
                    return minusOperation(a, b);
                else
                    return min;
            }
            public static T GeneralCheckedAdd<T>(T a, T b, Func<T, T, T> addOperation, Func<T, T, T> minusOperation, T max)
                where T : IComparable<T>
            {
                T different = minusOperation(max, a);
                if (different.CompareTo(b) != -1)
                    return addOperation(a, b);
                else
                    return max;
            }

        }



    }
}
