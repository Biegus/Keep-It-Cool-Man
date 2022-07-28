using System;
using System.Collections.Generic;
using Cyberultimate;
using UnityEngine.EventSystems;
namespace LetterBattle.Utility
{
    /// <summary>
    /// True if at least one registered
    /// </summary>
    public class QueueBool
    {
        private HashSet<object> manualyRegistered = new HashSet<object>();
        public event EventHandler<BoolResolverArgs> Event = delegate { };
        public void RegisterObj(object obj)
        {
            manualyRegistered.Add(obj);
        }
        public bool Unregister(object obj) { return manualyRegistered.Remove(obj);}
        public bool ContainsManualRegistered(object obj) { return manualyRegistered.Contains(obj); }
        public bool Evaluate()
        {
            if (manualyRegistered.Count >0) return true;
            BoolResolverArgs args = new BoolResolverArgs();
            Event(this, args);
            return !args.NoSignal;
        }
        public static implicit operator bool(QueueBool value)
        {
            return value.Evaluate();
        }
    }
}