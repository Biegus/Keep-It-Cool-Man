using System;
using System.Collections.Generic;
namespace Utility
{
    public  class AskerEventArgs<TEvent,TRequiredType> : EventArgs
        where TRequiredType: class
    {

        protected LinkedList<(TRequiredType obj, EventHandler<TEvent> ev)> registered = new LinkedList<(TRequiredType obj, EventHandler<TEvent> ev)>();
        public IReadOnlyCollection<(TRequiredType obj, EventHandler<TEvent> ev)> Registered => registered;

      
        public void Ask(TRequiredType obj, EventHandler<TEvent> ev)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (ev == null) throw new ArgumentNullException(nameof(ev));
            registered.AddLast((obj, ev));
        }
    }
    
  
}