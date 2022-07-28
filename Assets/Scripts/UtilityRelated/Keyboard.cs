using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace LetterBattle
{
    [Serializable]
    public class Keyboard: ISerializationCallbackReceiver
    {
        [SerializeField]
        private string keys;
        public string Keys => keys;
        private Dictionary<char, int> lookUpTable;

        public Keyboard(string keys)
        {
            this.keys = keys;
            RefreshDict();
        }
        private void RefreshDict()
        {
            lookUpTable = Keys.Select((item, i) => new KeyValuePair<char, int>(char.ToUpper(item), i)).ToDictionary(item => item.Key, item => item.Value);
        }
        public bool Has(char key)
        {
            return lookUpTable.ContainsKey(key);
        }
        public int GetIndex(char key)
        {
            if (lookUpTable.TryGetValue(key, out int value))
            {
                return value;
            }
            return -1;
        }
         void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            
        }
         void ISerializationCallbackReceiver.OnAfterDeserialize()
         {
            RefreshDict();;
         }
    }
}