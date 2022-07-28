using System;
using System.Collections;
using System.Collections.Generic;
using Cyberultimate;
using UnityEngine;
namespace LetterBattle
{
    [Serializable]
    public struct DirectionPackage<T>: IEnumerable<KeyValuePair<SimpleDirection,T>>
    {
        [SerializeField]
        private T up;
        [SerializeField]
        private T down;
        [SerializeField]
        private T left;
     
        [SerializeField]
        private T right;
        public T Down
        {
            get => down;
            set => down = value;
        }
        public T Left
        {
            get => left;
            set => left = value;
        }
        public T Up
        {
            get => up;
            set => up = value;
        }
        public T Right
        {
            get => right;
            set => right = value;
        }
        public  T this[SimpleDirection index]
        {
            get => index switch
            {
                SimpleDirection.Down => Down,
                SimpleDirection.Up => Up,
                SimpleDirection.Left => Left,
                SimpleDirection.Right => Right,
                _ => throw new ArgumentException()
            };
            set
            {
                _=index switch
                {
                    SimpleDirection.Down => Down=value,
                    SimpleDirection.Up => Up=value,
                    SimpleDirection.Left => Left=value,
                    SimpleDirection.Right => Right=value,
                    _ => throw new ArgumentException()
                };
            } 
        }

        public IEnumerator<KeyValuePair<SimpleDirection, T>> GetEnumerator()
        {
            for (int i = 0; i < 4; i++)
            {
                yield return new KeyValuePair<SimpleDirection, T>((SimpleDirection)(1<<i), this[(SimpleDirection)(1<<i)]);
            };
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}