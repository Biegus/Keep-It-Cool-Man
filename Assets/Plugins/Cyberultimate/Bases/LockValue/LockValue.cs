#nullable enable
using Cyberultimate;
using Cyberultimate.Internal;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;


namespace Cyberultimate
{
	public class LockValue<T>
		where T : struct, IComparable<T>
	{
		public enum Action
		{
			Add,
			Take
		}
		public enum Element
		{
			Value,
			Max
		}
		public interface ILockArgs
		{
			Action Action { get; }
			LockValue<T> LockValue { get; }
			T ValueBeforeChange { get; }
			T ValueAfterChange { get; }
		}
		public class AnyValueChangedArgs : EventArgs, ILockArgs
		{
			public T Last { get; }
			public T Actual { get; }
			public Action Action { get; }
			public Element Element { get; }
			public object? From { get; }
			public LockValue<T> LockValue { get; }
			T ILockArgs.ValueAfterChange => Actual;
			T ILockArgs.ValueBeforeChange => Last;
			public AnyValueChangedArgs(T last, T actual, Action action, Element element, object? who, LockValue<T> lockValue)
			{
				Last = last;
				Actual = actual;
				Action = action;
				Element = element;
				From = who;
				LockValue = lockValue;
			}
		}
		public class CanChangeArgs : BoolResolverArgs, ILockArgs
		{
			public object? From { get; }
			public LockValue<T> LockValue { get; }
			public T TryTo { get; }
			public Action Action { get; }
			public T ValueBeforeChange { get; }
			T ILockArgs.ValueAfterChange => TryTo;
			public CanChangeArgs(LockValue<T> hp, T tryTo, Action action, object? from)
			{
				LockValue = hp ?? throw new ArgumentNullException(nameof(hp));
				TryTo = tryTo;

				Action = action;
				From = from;
				ValueBeforeChange = hp.Value;

			}
		}
		public readonly T MaxOfMax;
	
		public T Max => _Max;
		public T Min { get; }
		public T Value
		{
			get => _Value;
			private set
			{
				if (value.Equals(_Value))
					return;
				_Value = MathHelper.Generic.Clamp<T>(value, Min, Max);


			}
		}
		
		public event EventHandler<AnyValueChangedArgs> OnValueChanged = delegate { };
		public event EventHandler<AnyValueChangedArgs> OnValueTaken = delegate { };
		public event EventHandler<AnyValueChangedArgs> OnValueGiven = delegate { };
		public event EventHandler<AnyValueChangedArgs> OnValueChangedToMin = delegate { };
		public event EventHandler<AnyValueChangedArgs> OnMaxValueChanged = delegate { };
		public event EventHandler<CanChangeArgs> CanChange = delegate { };

		private T _Value = default;
		private T _Max;
		
		private readonly ILockValuePacker<T> packer;
		
		public LockValue(T max, T min, T value, T? maxOfMax = null)
		{
			_Max = max;
			Min = min;
			Value = value;
			MaxOfMax = maxOfMax ?? max;
			packer = Cyberultimate.Internal.LockValuePackersFactory.Create<T>();
		}
		public (T min, T max) GetRange() { return (Min, Max); }
		public void SetValue(T val, object? from=null)
		{
			T before = this.Value;
			if (val.Equals(Value))
				return;
			Action action = (val.CompareTo(Value) == 1) ? Action.Add : Action.Take;

			var args = new CanChangeArgs(this, val, action, @from);
			CanChange(this, args);
			if (args.NoSignal)
			{
				
				T last = Value;
				Value = val;
				AnyValueChangedArgs ev = new AnyValueChangedArgs(last, Value, action, Element.Value, from, this);
				OnValueChanged(this, ev);
				if (Value.Equals(Min)&& !before.Equals(Value))
					OnValueChangedToMin(this, ev);
				switch (action)
				{
					case Action.Add:
						OnValueGiven(this, ev); break;
					case Action.Take:
						OnValueTaken(this, ev); break;
				}
			}

		}
		public void SetMax(T val, object? who=null)
		{
			T last = _Max;
			if (_Max.Equals(val))
				return;
			val = MathHelper.Generic.Clamp<T>(val, packer.GetMin(), MaxOfMax);
			_Max = val;
			if (_Value.CompareTo(_Max) == 1)
			{
				SetValue(_Max, "Max");
			}
			OnMaxValueChanged.Invoke(this, new AnyValueChangedArgs(last, val, (last.CompareTo(val) == 1) ? Action.Take : Action.Add, Element.Max, who, this));
		}

		public void TakeValue(T value, object? from=null)
		{
			SetValue(packer.Remove(Value, value), from);
		}
		public void GiveValue(T value, object? from=null)
		{
			SetValue(packer.Add(Value, value), from);
		}
		public bool IsInRange(T val)
			=> Max.CompareTo(val) != -1 && Min.CompareTo(val) != 1;
	

		public override string ToString()
		{
			return $"{Value}/{Max}";
		}


	}
}
