#nullable enable
using System;
using System.Collections.Generic;
using Cyberultimate.Unity.Internal;

namespace Cyberultimate.Unity
{
    namespace Internal
    {
        public interface ICyberCoroutineInternal
        {
            internal bool InternalIsDone();
            internal void InternalMakeItDone();
            internal void IntenalSetException(Exception exception);
            internal void InternalConfDeath(CorFinishState state);
            internal CorRaw? InternalGetRaw();

        }
    }
 
    public  class CyberCoroutine: IWaitableCondictional, Internal.ICyberCoroutineInternal
    {

        private CorRaw? raw;//this will be null after the end of cor
        public CorController Controller { get; }
        public CyberCorState State => raw?.State ?? CyberCorState.Ended;
        public Exception? Exception { get; private set; }
        public CorFinishState WayOfStopping { get; private set; }
    
        public bool IsExceptionSupressed
        {
            get
            {
                ThrowIfFullyStopped("Checking suppression of exception");

                return raw!.InnerStates.HasFlag(CorRaw.InnerStateFlags.IsExceptionSuppressed);
            }
            set
            {
                ThrowIfFullyStopped("Setting suppression of exception");
               
                if (value)
                {
                    raw!.InnerStates |= CorRaw.InnerStateFlags.IsExceptionSuppressed;
                }
                else
                {
                    raw!.InnerStates ^= (raw.InnerStates & CorRaw.InnerStateFlags.IsExceptionSuppressed);
                }
            }
        }
        public event EventHandler<CyberCoroutine> OnFinished
        {
            add=>  InvokeEvent((events =>events.OnFinished+=value));
            remove=> InvokeEvent((events =>events.OnFinished-=value));
        }
        public event EventHandler<CyberCoroutine> OnPause
        {
            add=>  InvokeEvent((events =>events.OnPaused+=value));
            remove=> InvokeEvent(events =>events.OnPaused-=value);
        }
        public event EventHandler<CyberCoroutine> OnResume
        {
            add=>  InvokeEvent((events =>events.OnResumed+=value));
            remove=> InvokeEvent((events =>events.OnResumed-=value));
        }
        
        public CyberCoroutine(IEnumerator<IWaitable> enumerator,CorController controller, UnityEngine.Object? limit)
        {
            if (enumerator == null) throw new ArgumentNullException(nameof(enumerator));
            raw = ((IInternalController) controller).InternalRegister(this, enumerator,limit);
            Controller = controller;
        }

        
        public void Pause()
        {
            raw?.Pause();
        }

        public void Resume()
        {
            raw?.Resume();
        }
        /// <summary>
        /// Stops entirely, if you want to Pause, use <see cref="Pause"/>
        /// </summary>
        public void Stop()
        {
            raw?.Stop(CorFinishState.ManualStopped);
        }

        public CyberCoroutine OnEnd(Action<CyberCoroutine> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            OnFinished +=(s,e)=> action(e);
            return this;
        }

        public CyberCoroutine OnEnd(Action<CyberCoroutine> action, CorFinishState required)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            OnFinished +=(s,e)=>
            {
                if (e.WayOfStopping == required)
                    action(e);
            };
            return this;
        }

       
        bool IWaitableCondictional.IsReady()
        {
            return ((this as Internal.ICyberCoroutineInternal)).InternalIsDone() ;
        }

        void IWaitableCondictional.Pause()
        {
           
        }
        void IWaitableCondictional.Resume()
        {
           
        }

         CorRaw? Internal.ICyberCoroutineInternal.InternalGetRaw()
        {
            return raw;
        }
        private void ThrowIfFullyStopped(string action)
        {
            if (raw == null)
                throw new InvalidOperationException(
                    $"This action {action} cannot be performed on fully stopped coroutine");
        }
        private void InvokeEvent(Action<CorRaw.Events> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (raw != null)
            {
                action(raw.EventsElement);
            }
        }

        bool Internal.ICyberCoroutineInternal.InternalIsDone() => raw == null;
        void Internal.ICyberCoroutineInternal.InternalMakeItDone()
        {
            raw = null;
        }
        void Internal.ICyberCoroutineInternal.IntenalSetException(Exception exception)
        {
            this.Exception = exception;
        }

        void Internal.ICyberCoroutineInternal.InternalConfDeath(CorFinishState state)
        {
            WayOfStopping = state;
        }
    }
}