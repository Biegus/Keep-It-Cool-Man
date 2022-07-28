#nullable enable
using Cyberultimate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cyberultimate
{
    /// <summary>
    /// <see cref="Double"/> decorator, acceptable is only value in 0-1 range.
    /// Should be use rather only for serialize fields.
    /// </summary>
    [Serializable]
    public struct Percent : IComparable<Percent>, IComparable<float>, IComparable, IEquatable<float>, IEquatable<Percent>
    {
        public const float MaxValue = 1.0f;
        public const float MinValue = 0.0f;
        public static readonly Percent Zero = new Percent();
        public static readonly Percent Half = new Percent(0.5f);
        public static readonly Percent Full = new Percent(1);
        [SerializeField]
        private  float _Value;


        /// <summary>
        /// Returns value in 0-1 range.
        /// </summary
        public float Value => _Value;

        /// <summary>
        /// Returns value in 0-100 range.
        /// </summary>
        public byte AsByte => (byte)(_Value * 100);

        public Percent(float @decimal)
        {

            _Value = Percent.Clamp(@decimal);
        }
        /// <summary>
        /// Returns non-abs difference.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public float Difference(Percent other)
           => this.Value - other.Value;
        public static bool operator ==(Percent a, Percent b)
            => a.Value == b.Value;
        public static bool operator !=(Percent a, Percent b)
            => !(a == b);
        public static bool operator >(Percent a, Percent b)
            => a.Value > b.Value;
        public static bool operator <(Percent a, Percent b)
            => a.Value < b.Value;
        public static bool operator <=(Percent a, Percent b)
            => a == b || a < b;
        public static bool operator >=(Percent a, Percent b)
          => a == b || a > b;
        public static explicit operator double(Percent procent)
            => procent.Value;
        public static explicit operator float(Percent procent)
            => procent.Value;
        public static Percent operator +(Percent a, float b)
         => new Percent(Math.Min(1, a.Value + b));
        public static Percent operator -(Percent a, float b)
        => new Percent(Math.Max(0, a.Value - b));
        public static Percent operator *(Percent a, float b)
            => new Percent(Math.Min(1, a.Value * b));
        public static Percent operator /(Percent a, float b)
            => new Percent(Math.Min(1, a.Value / b));
        public static Percent operator +(Percent a, Percent b)
            => a + b.Value;
        public static Percent operator -(Percent a, Percent b)
            => a - b.Value;
        public static Percent operator *(Percent a, Percent b)
            => a * b.Value;
        public static Percent operator /(Percent a, Percent b)
            => a / b.Value;

        public override bool Equals(object obj)
        {
            if (obj is Percent p)
                return this == p;
            else return false;
        }
        public override string ToString()
        {
            return $"{Value * 100}%";
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        int IComparable<float>.CompareTo(float other)
        {
            return _Value.CompareTo(other);
        }

        int IComparable.CompareTo(object obj)
        {
            return _Value.CompareTo(obj);
        }

        bool IEquatable<float>.Equals(float other)
        {
            return _Value.Equals(other);
        }

        int IComparable<Percent>.CompareTo(Percent other)
        {
            return _Value.CompareTo(other._Value);
        }

        bool IEquatable<Percent>.Equals(Percent other)
        {
            return _Value.Equals(other._Value);
        }
        private static float Clamp(float val)
        {
            return Math.Max(Math.Min(val, 1f), 0f);
        }
        /// <summary>
        /// Creates a percent by a byte value in 0-100 range.
        /// </summary>
        /// <param name="val"></param>
        public static Percent FromByte(byte val)
        {
            return new Percent(val / 100.0f);
        }
        /// <summary>
        /// Creates a percent by a double value in 0-1 range.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static Percent FromDecimal(float val)
        {
            return new Percent(val);
        }
       
        public static Percent FromValueInRange(float value, (float min, float max) range)
        {
            return Percent.FromDecimal(MathHelper.ReCalculateRange(range, (0, 1), value));
        }
        
        public static Percent Parse(string percent)
        {
            return Percent.FromDecimal(float.Parse(percent.Replace("%", "")) / 100);
        }


    }


}

