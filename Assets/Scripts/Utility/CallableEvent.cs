using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetterBattle.Utility;
using UnityEngine;
namespace LetterBattle
{
    public struct EventAction
    {
        public BaseCallableEvent Ev;
        public Delegate Func;
        public EventAction(BaseCallableEvent ev, Delegate func)
        {
            Ev = ev;
            Func = func;
        }
        public void Unregister()
        {
            Ev.UnregisterNonType(Func);
        }
        

    }
  
    public abstract class BaseCallableEvent
    {
        
        public static event EventHandler<EventAction> OnAnyEventRegistered = delegate { };
        public static event EventHandler<EventAction> OnAnyEventUnregistered = delegate { };
        protected  void CallOnAnyEventRegistered(Delegate @delegate)
        {
            OnAnyEventRegistered(null, new EventAction(this,@delegate));
        }
        protected void CallOnAnyEventUnregistered(Delegate @delegate)
        {
            OnAnyEventUnregistered(null, new EventAction(this,@delegate));
        }
        public abstract void RegisterNonType(Delegate func);
        public abstract void UnregisterNonType(Delegate @delegate);
        public abstract IEnumerable<Delegate> GetInvocationList();
        public abstract void CallNonObject(object val);
        public abstract int GetCallsCount();



    }
   
    public class CallableEvent<T> : BaseCallableEvent
    {
        public event EventHandler<T> Raw = delegate { };
        public event Action RawAction = delegate { };
       
        public QueueValue<bool> DeafStatus { get; } = QueueBoolBuilder.Build();
        public void Call(T args)
        {
            if (DeafStatus.Value) return;
            EventHelper.InvokeEventSafe(()=>Raw(this,args));
            EventHelper.InvokeEventSafe(()=>RawAction());
			
        }
        public void Register(EventHandler<T> action)
        {
            Raw += action ?? throw new ArgumentNullException(nameof(action));
            CallOnAnyEventRegistered(action);

        }
        public void Register(Action action)
        {
            RawAction += action ?? throw new ArgumentNullException(nameof(action));
            CallOnAnyEventRegistered(action);

        }
        public void Unregister(EventHandler<T> action)
        {
            Raw -= action ?? throw new ArgumentNullException(nameof(action));
            CallOnAnyEventUnregistered(action);
        }
        public void Unregister(Action action)
        {
            RawAction -= action ?? throw new ArgumentNullException(nameof(action));
            CallOnAnyEventUnregistered(action);
        }
        public override void RegisterNonType(Delegate func)
        {
            if (func is Action action)
            {
                Register(action);
            }
            else
                Register((EventHandler<T>)(func));
        }
        public override void UnregisterNonType(Delegate @func)
        {
            if (func is Action action)
            {
                Unregister(action);
            }
            else
                Unregister((EventHandler<T>)(func));
        }
        public override IEnumerable<Delegate> GetInvocationList()
        {
            return Raw.GetInvocationList().Skip(1).Concat(RawAction.GetInvocationList().Skip(1));
        }
        public override void CallNonObject(object val)
        {
            if(val==null)
                Call(default);
            Call((T)val);
        }
        public override int GetCallsCount()
        {
            return Raw.GetInvocationList().Length + RawAction.GetInvocationList().Length - 2;
        }
    }
    public static class CallableEventMarker
    {
        
        private static readonly Dictionary<object, LinkedList<EventAction>> dict = new Dictionary<object, LinkedList<EventAction>>();
        private static object current = null;
        static CallableEventMarker()
        {
            BaseCallableEvent.OnAnyEventRegistered += OnAnyEventRegistered;
        }
        private static void OnAnyEventRegistered(object sender, EventAction action)
        {
            if (current != null)
                dict[current].AddLast(action);
        }
        private static void InternalEnd(object obj)
        {
            if(dict.TryGetValue(obj, out var result))
            {
                foreach (var element in result)
                {
                    element.Unregister();
                }
            }
        }
        public static void Start(object obj)
        {
            if (current != null)
            {
                throw new InvalidOperationException("Start cannot be called until start before is disposed");
            }
            if(!dict.ContainsKey(obj))
                dict[obj] = new LinkedList<EventAction>();
            current = obj;

        }
        public static void DoInStartSpace(object obj, Action action)
        {
            Start(obj);
            action();
            EndStart(obj);
        }
        public static void EndStart(object obj)
        {
            current = null;
        }
        public static void End(object obj)
        {
            InternalEnd(obj);
            dict.Remove(obj);
        }
    }
    

}