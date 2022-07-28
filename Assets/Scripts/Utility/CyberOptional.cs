using System;
using UnityEngine;
namespace LetterBattle
{
    [Serializable]
    public struct CyberOptional<T>
    {
        public static CyberOptional<T> Empty { get; }=  new CyberOptional<T>();
        
        [SerializeField]
        private bool custom;
        [SerializeField]
        private T value;
        public CyberOptional(T value)
        {
            this.custom = true;
            this.value = value;
        }

        public bool HasValue() { return custom;}
        public T GetValue(T def=default)
        {
            if (!custom) return def;
            else return value;
        }

    }
}