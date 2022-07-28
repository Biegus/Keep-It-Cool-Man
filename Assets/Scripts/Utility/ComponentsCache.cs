using System;
using System.Collections.Generic;
using UnityEngine;
namespace LetterBattle.Utility
{
    public class ComponentsCache
    {
        private readonly Dictionary<Type, Component> dict = new Dictionary<Type, Component>();
        public GameObject GameObject { get; }
        public T Get<T>()
            where T:class
           
        {
            if (dict.TryGetValue(typeof(T), out var value))
            {
                return (T)(object)value;
            }
            T comp = GameObject.GetComponent<T>();
            dict[typeof(T)] = (Component)(object)comp;
            return comp;
        }
        public T GetOrAdd<T>()
            where T:Component
        {
            T cached = Get<T>();
            if ((cached) != null)
                return cached;
                    
            this.GameObject.AddComponent<T>();
            return Get<T>();

        }
        public ComponentsCache(GameObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            this.GameObject = obj;
        }
    }
}