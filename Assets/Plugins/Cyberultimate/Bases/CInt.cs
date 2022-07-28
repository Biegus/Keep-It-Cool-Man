#nullable enable
#if UNITY_EDITOR || UNITY_STANDALONE ||UNITY_WII|| UNITY_IOS || UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN ||UNITY_TVOS || UNITY_WSA || UNITY_WSA_10_0 || UNITY_WINRT || UNITY_WINRT_10_0 ||UNITY_WEBGL ||UNITY_FACEBOOK||UNITY_FACEBOOK||UNITY_ADS || UNITY_ANALYTICS ||UNITY_ASSERTIONS ||UNITY_64
#define ANY_UNITY
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Cyberultimate
{

   /// <summary>
   /// Checked uint. It's slower than just uint.
   /// </summary>
    [Serializable]
    public struct Cint : IComparable, IFormattable, IConvertible, IComparable<UInt32>, IEquatable<UInt32>, IComparable<Cint>, IEquatable<Cint>
    {
        public static readonly Cint Zero = new Cint();
       
        public static readonly Cint MaxValue = uint.MaxValue;
#if  ANY_UNITY
        [UnityEngine.SerializeField]
#endif
        private uint _Value;
        
        public Cint(uint value = 0)
        {
            this._Value = value;
        }
        public static Cint Parse(string text)
            => uint.Parse(text);
        public static Cint operator +(Cint a, Cint b)
        {
            return Cint.CheckedAdd(a, b);
        }
        public static Cint operator -(Cint a, Cint b)
        {
            return Cint.CheckedRemove(a, b);
        }
        public static Cint operator *(Cint a, Cint b)
        {
            return Cint.CheckedMultiple(a, b);
        }
        public static double operator*(Cint a, double b)
        {
            return a._Value * b;
        }
        public static Cint operator /(Cint a, Cint b)
        => a._Value / b._Value;
        public static Cint operator ++(Cint a)
            => a + new Cint(1);
        public static Cint operator --(Cint a)
           => a - new Cint(1);
        public static Cint operator <<(Cint a, int n)
            => a._Value << n;
        public static Cint operator >>(Cint a, int n)
            => a._Value >> n;
        public static implicit operator Cint(uint value)
            => new Cint(value);
        public static implicit operator uint(Cint value)
            => value._Value;


      
        public static uint CheckedAdd(uint a, uint b)
        {
            uint different = uint.MaxValue - a;
            if (different >= b)
                return a + b;
            else
                return uint.MaxValue;
        }
      
       
        public static uint CheckedRemove(uint a,uint b)
        {
            uint different = a - uint.MinValue;
            if (different >= b)
                return a - b;
            else
                return uint.MinValue;
        }
   
        public static uint CheckedMultiple(uint a, uint b)
        {
            if (a == 0)
                return 0;
            uint maxMultiple = uint.MaxValue / a;
            if (b >= maxMultiple)
                return (a * b);
            else
                return uint.MaxValue;
        }
       
     
        public override string ToString()
        {
            return _Value.ToString();
        }
        public int CompareTo(object obj)
        {
            return _Value.CompareTo(obj);
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return _Value.ToString(format, formatProvider);
        }
        public TypeCode GetTypeCode()
        {
            return _Value.GetTypeCode();
        }
        public bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)_Value).ToBoolean(provider);
        }
        public char ToChar(IFormatProvider provied)
        {
            return ((IConvertible)_Value).ToChar(provied);
        }
        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)_Value).ToSByte(provider);
        }
        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)_Value).ToByte(provider);
        }
        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)_Value).ToInt16(provider);
        }
        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)_Value).ToUInt16(provider);
        }
        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)_Value).ToInt32(provider);
        }
        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)_Value).ToUInt32(provider);
        }
        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)_Value).ToInt64(provider);
        }
        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)_Value).ToUInt64(provider);
        }
        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)_Value).ToSingle(provider);
        }
        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)_Value).ToDouble(provider);
        }
        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)_Value).ToDecimal(provider);
        }
        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)_Value).ToDateTime(provider);
        }
        public string ToString(IFormatProvider provider)
        {
            return _Value.ToString(provider);
        }
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)_Value).ToType(conversionType, provider);
        }
        public int CompareTo(uint other)
        {
            return _Value.CompareTo(other);
        }
        public bool Equals(uint other)
        {
            return _Value.Equals(other);
        }
        public override bool Equals(object obj)
        {
            return _Value.Equals(obj);
        }
        public int CompareTo(Cint other)
        {
            return other._Value.CompareTo(other._Value);
        }
        public bool Equals(Cint other)
        {
            return other._Value.Equals(other._Value);
        }
        public override int GetHashCode()
        {
            return _Value.GetHashCode();
        }
    }
}

