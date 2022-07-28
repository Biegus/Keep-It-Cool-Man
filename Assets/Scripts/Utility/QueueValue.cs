using System;
using System.Collections.Generic;
using System.Linq;
namespace LetterBattle.Utility
{

    
    public class QueueBoolBuilder
    {
        public static QueueValue<bool> Build()
        {
            return new QueueValue<bool>(false, (a, b) => a || b);
        }
    }
    
    
    /// <summary>
    /// Value that is the most "strong" of it's values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueueValue<T>
    {
        
        //TODO: if no one subscribes, then do reset cache only when necessary
        
        public delegate T DeciderCallback(T a, T b);
        public readonly T Identity;
        
        public T Value => cacheValue;
        public int RegisteredAmount => registered.Count;
        public DeciderCallback DeciderFunction { get; }
        
        private Dictionary<object, T> registered = new Dictionary<object, T>();
        public event EventHandler<QueueValue<T>> OnValueChanged = delegate { };
        
        private T cacheValue;
        public QueueValue(T identity,  DeciderCallback decider)
        {
            this.Identity = identity;
            this.DeciderFunction = decider ?? throw new ArgumentNullException(nameof(decider));
            this.cacheValue = identity;

        }
        public void Register(object obj,T value)
        {
            if (value.Equals(Identity)|| registered.ContainsKey(obj))
            {
                Unregister(obj);
            }
            if (value.Equals(Identity)) return;
            
            registered[obj] = value;
            T newValue = DeciderFunction(cacheValue, value);// the cache value has the most priority, if the new value beats, it beats everything
            TryChangeValue(newValue);
        }
        public bool Unregister(object obj)
        {
            if (!registered.ContainsKey(obj))
                return false;
            T value = registered[obj];
            registered.Remove(obj);
            if (value.Equals(cacheValue))//only case in which everything should be calculated from the beginning
            {
                T nwValue= InternalGetValue();
                TryChangeValue(nwValue);
            }
            return true;
            // if the cache value is different than value that means that the "cache value" has more priority over "value", thus removing value
            //will not change anything
        }
        
        //calculate the value from beginning
        private T InternalGetValue()
        {
            return registered.Aggregate(Identity, (current, element) => DeciderFunction(current, element.Value));
        }
       
        private void TryChangeValue(T value)
        {

            if (!value.Equals(cacheValue))
            {
                cacheValue = value;
                OnValueChanged(this, this);
            }
        }
      


    }
}