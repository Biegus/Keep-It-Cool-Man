#nullable enable
using System;
using System.Collections.Generic;


namespace Cyberultimate.Unity
{
    internal class CorRaw
        {
            [Flags]
            internal enum InnerStateFlags:byte
            {
                None=0,
                HasLimit=1<<0,
                HasEscaped=1<<2,
                HasReturned=1<<3,
                HasReturnedAndIsFinished=1<<4,
                IsExceptionSuppressed=1<<5,
                
            }
            internal class Events
            {
                public EventHandler<CyberCoroutine> OnFinished { get; set; } = delegate { };
                public  EventHandler<CyberCoroutine> OnPaused { get; set; } = delegate { };
                public EventHandler<CyberCoroutine> OnResumed { get; set; } = delegate { };
                public EventHandler<CyberCoroutine> OnExceptionThrown { get; set; } = delegate { };
            }
            public CyberCoroutine Coroutine { get; }
            public Stack<IEnumerator<IWaitable>> Enumerator { get; } = new Stack<IEnumerator<IWaitable>>();
            public IWaitable? Current { get; set; }
            public CyberCorState State { get; set; } = CyberCorState.Running;
            public UnityEngine.Object? Limit { get; set; }
            public InnerStateFlags InnerStates { get; set; }
            public LinkedListNode<CyberCoroutine>? Node { get; set; }
            public Events EventsElement { get; } = new Events();
            public CorLoopState LoopState { get; set; } = CorLoopState.Update;
            public CorRaw(CyberCoroutine cor)
            {
                Coroutine = cor;
            }
            public bool CanMove() => State == CyberCorState.Running && (Current == null ||
                                                                        ((Current as IWaitableCondictional)
                                                                            ?.IsReady() ?? true))
                                                                    && !InnerStates.HasFlag(InnerStateFlags.HasEscaped)
                                                                    && LoopState == Coroutine.Controller.CurrentLoop;
            public bool IsLimitOver()
            {
                return Limit == null && InnerStates.HasFlag(InnerStateFlags.HasLimit) == false;
            }
            public void Pause()
            {
            
                CheckForPausing();
                if (this.State != CyberCorState.Running)
                    return;
                this.State = CyberCorState.Paused;
                this.EventsElement.OnPaused(Coroutine, Coroutine);
                (this.Current as IWaitableCondictional)?.Pause();
            }

            private void CheckForPausing()
            {
                if (Coroutine == null)
                    throw new ArgumentNullException(nameof(Coroutine));
                if (this.State == CyberCorState.Ended)
                    throw new InvalidOperationException("Coroutine has finishe route");
            }
       
            public void Resume()
            {
                CheckForPausing();
                if (this.State != CyberCorState.Paused)
                    return;
                this.State = CyberCorState.Running;
                this.EventsElement.OnResumed(Coroutine, Coroutine);
                (this.Current as IWaitableCondictional)?.Resume();
            }
           
           

            internal void Stop(CorFinishState corFinishState)
            {
                if (this.State == CyberCorState.Ended)
                    return;
                Current = null;
                State = CyberCorState.Ended;
                ((Internal.ICyberCoroutineInternal) Coroutine).InternalConfDeath(corFinishState);
                EventsElement.OnFinished(Coroutine, Coroutine);
                EventsElement.OnFinished = null; //this is not ever gonna be raise again, let the GC works
                EventsElement.OnPaused = null;
                EventsElement.OnResumed = null;
               
                
            }
            
        }
    
}