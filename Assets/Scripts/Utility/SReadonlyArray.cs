using System;
using System.Collections.ObjectModel;
using UnityEngine;
namespace LetterBattle.Utility
{
    //this array contains logic for readonly that is safe and efficient to work with 
    //unity serialized ones
    [Serializable]
    public class SReadonlyArray<T>: ISerializationCallbackReceiver
    {
      
        [ValidateRecursively]
        [SerializeField] private T[] array= new T[0];
        private T[] lastRef = null;
        private ReadOnlyCollection<T> rArray;
        public ReadOnlyCollection<T>Array => rArray??= new ReadOnlyCollection<T>(array);
        public  T this[int index] => array[index];
        public void OnBeforeSerialize()
        {
            return;
        }
        public void OnAfterDeserialize()
        {
            if (array!=lastRef)
            {
                rArray = null;// will be lazy reset after first try to access.
            }
            lastRef = array;

        }
    }
}