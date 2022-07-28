#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

namespace Cyberultimate.Internal
{
	public interface ILockValuePackerBase { }
	public interface ILockValuePacker<T>:ILockValuePackerBase
		where T:IComparable<T	>
	{
		T Add(T a, T b);
		T Remove(T a, T b);
		T GetMin();
	}

    internal abstract class SimplePacker<T> : ILockValuePacker<T>
		where T:IComparable<T>
    {
		protected abstract T SimpleAdd(T a, T b);
		public abstract T SimpleRemove(T a, T b);
		public abstract T Max { get; }
		public abstract T Min { get; }
		public T Add(T a, T b)
		{
			return MathHelper.Generic.GeneralCheckedAdd<T>(a, b, SimpleAdd, SimpleRemove, Max);
		}

		public T GetMin()
		{
			 return Min;
		}

		public T Remove(T a, T b)
		{
			return MathHelper.Generic.GeneralCheckedRemove<T>(a, b, SimpleRemove, Min);
		}
	}
    public static class LockValuePackersFactory
    {

	    internal class IntValuePacker : SimplePacker<int>
	    {
		    public override int Max => int.MaxValue;
		    public override int Min => int.MinValue;
		    protected override int SimpleAdd(int a, int b)
		    {
			    return a + b;
		    }
		    public override int SimpleRemove(int a, int b)
		    {
			    return a - b;
		    }
	    }
		internal class UIntValuePacker : ILockValuePacker<uint>
		{
			public uint Add(uint a, uint b)
			{
				return Cint.CheckedAdd(a, b);

			}
			public uint GetMin()
			{
				return uint.MinValue;
			}

			public uint Remove(uint a, uint b)
			{
				return Cint.CheckedRemove(a, b);
			}
		}
		internal class FloatValuePacker : SimplePacker<float>
		{
			public override float Max => float.MaxValue;

			public override float Min => float.MinValue;
			public override float SimpleRemove(float a, float b)
			{
				return a - b;
			}
			protected override float SimpleAdd(float a, float b)
			{
				return a + b;
			}
		}
		internal class DoubleValuePacker : SimplePacker<double>
		{
			public override double Max => double.MaxValue;

			public override double Min => double.MinValue;

			public override double SimpleRemove(double a, double b)
			{
				return a - b;
			}

			protected override double SimpleAdd(double a, double b)
			{
				return a + b;
			}
		}

		private static readonly Dictionary<Type, ILockValuePackerBase> packers = null;
		static LockValuePackersFactory()
		{
			packers = (
			 from item in AppDomain.CurrentDomain.GetAssemblies().SelectMany(item => item.GetTypes())
			 let interfc = item.GetInterfaces().FirstOrDefault(element => element.IsGenericType&&element.GetGenericTypeDefinition() == typeof(ILockValuePacker<>)) 
			
			 where interfc!= null && item.IsAbstract == false&&item.IsInterface==false
			 select (interfc.GetGenericArguments()[0], (ILockValuePackerBase)Activator.CreateInstance(item)))
			.ToDictionary(item => item.Item1, item => item.Item2);
        }
		public static ILockValuePacker<T> Create<T>()
			where T:IComparable<T>
        {
			if(packers.TryGetValue(typeof(T),out var value))
            {
				return (ILockValuePacker<T>)value;	
			
            }
			throw new ArgumentException("Not supported type");
        }
    }
}

