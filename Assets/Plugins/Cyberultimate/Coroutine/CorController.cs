#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;

using Cyberultimate.Unity.Internal;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cyberultimate.Unity
{
    namespace Internal
    {
        public interface IInternalController
        {
            internal CorRaw InternalRegister(CyberCoroutine coroutine, IEnumerator<IWaitable> enumerator, UnityEngine.Object? limit);
        }
    }
    public enum CorLoopState
    {
        Unknown=0,
        Update,
        Fixed,
        Late
    }
    public  class CorController: Internal.IInternalController
    {
        public static CorController Base => InternalCyberCoroutineController.Current!.Controller;
        
        public CorLoopState CurrentLoop { get; private set; }
        private readonly LinkedList<CyberCoroutine> aliveList = new LinkedList<CyberCoroutine>();
        private readonly LinkedList<CyberCoroutine> deathList = new LinkedList<CyberCoroutine>();
        private readonly LinkedList<CyberCoroutine> birthList = new LinkedList<CyberCoroutine>();
        
        public CyberCoroutine Start(IEnumerator<IWaitable> enumerator,UnityEngine.Object? limit)
        {
            if (enumerator == null) throw new ArgumentNullException(nameof(enumerator));
            return new CyberCoroutine(enumerator, this, limit);
        }

        public ValueCoroutine<T> StartWithValue<T>(IEnumerator<IWaitable> enumerator, UnityEngine.Object limit)
        {
            if (enumerator == null) throw new ArgumentNullException(nameof(enumerator));
            return new ValueCoroutine<T>(enumerator, this, limit);
        }
        
        public void UpdateState(CorLoopState state)
        {
            if (CorLoopState.Unknown==state)
                throw new ArgumentException("State is not correct");
            RaiseNewest();

            this.CurrentLoop = state;
            
            foreach (var element in aliveList)
            {
                InternalUpdate(element);
            }
            this.CurrentLoop  = CorLoopState.Unknown;
            ClearDeadOnes();
        }

      
        private void RaiseNewest()
        {
            foreach (CyberCoroutine element in birthList)
            {
                ((ICyberCoroutineInternal)element).InternalGetRaw()!.Node = aliveList.AddLast(element);
            }

            birthList.Clear();
        }
        public CyberCoroutine DelayOneFrame(Action act ,Object? limit)
        {
            if (act == null) throw new ArgumentNullException(nameof(act));

            IEnumerator<IWaitable> Core()
            {
                yield return Yield.NextFrame;
                act();
            }
            return this.Start(Core(), limit);
        }

        private void ClearDeadOnes()
        {
            foreach (var element in deathList)
            {
                var raw = ((ICyberCoroutineInternal) element).InternalGetRaw();
                if (raw!.Node != null)
                {
                    aliveList.Remove(raw.Node);
                    raw.Node = null;
                }
                ((Internal.ICyberCoroutineInternal) element).InternalMakeItDone();
            }
            deathList.Clear();
        }

        private void PushOnStack(CyberCoroutine cor,IEnumerator<IWaitable> element)
        {
            ((ICyberCoroutineInternal) cor).InternalGetRaw()!.Enumerator.Push(element);
        }

        private void Stop(CyberCoroutine cor,CorFinishState state)
        {
            ((ICyberCoroutineInternal) cor).InternalGetRaw()?.Stop(state);
            InternalFinish(cor);
        }
        
        internal void InternalFinish(CyberCoroutine coroutine)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));
            deathList.AddLast(coroutine);
        }

        private void InternalUpdate(CyberCoroutine coroutine)
        {
            CorRaw data = ((ICyberCoroutineInternal) coroutine).InternalGetRaw();
            if (!data.CanMove()) return;
            var en = data.Enumerator.Peek();
            bool goNext;
            bool pop = false;
            goNext = !data.IsLimitOver();
            if (!goNext)
            {
                data.Stop(CorFinishState.NaturallyStopped);
                return;
            }
          

             if (data.InnerStates.HasFlag(CorRaw.InnerStateFlags.HasReturned))
             {
                 pop = false;
                 data.InnerStates ^= CorRaw.InnerStateFlags.HasReturned;
             }
             else if (data.InnerStates.HasFlag(CorRaw.InnerStateFlags.HasReturnedAndIsFinished))
             {
                 pop = true;
                 data.InnerStates ^= CorRaw.InnerStateFlags.HasReturnedAndIsFinished;
             }
             else
             {
                 try
                 {
                     pop = !en.MoveNext();
                 }
                 catch(Exception exception)
                 {
                     goNext = false;
                     if (!data.InnerStates.HasFlag(CorRaw.InnerStateFlags.IsExceptionSuppressed))
                         Debug.LogException(exception);
                     ((Internal.ICyberCoroutineInternal) data.Coroutine).IntenalSetException(exception);
                     data.Stop(CorFinishState.StoppedByException);
                 }
            
             }
            

            if (!pop)
            {
                switch (en.Current)
                {
                    case CorReturn coReturn when data.Enumerator.Count == 1:
                        goNext = false;
                        (coroutine as Internal.IValueCoroutineInternal)?.InternalSetResult(coReturn.Value);
                        break;
                    case StateChange stateChange:
                        data.LoopState = stateChange.Loop;
                        break;
                    case WaitablePush push:
                        PushOnStack(coroutine, push.Element);
                        break;
                    case EscapeThread escape:
                        data.InnerStates |= CorRaw.InnerStateFlags.HasEscaped;
                        if (escape.UseThreadPool)
                            Task.Run(DoOnOtherThread);
                        else
                            new Thread(DoOnOtherThread).Start();

                        void DoOnOtherThread()
                        {
                            var res = en.MoveNext();
                            data.InnerStates ^= CorRaw.InnerStateFlags.HasEscaped; //byte is atomic
                            if (res)
                                data.InnerStates |= CorRaw.InnerStateFlags.HasReturned;
                            else
                                data.InnerStates |= CorRaw.InnerStateFlags.HasReturnedAndIsFinished;
                        }

                        return;
                    
                    

                }
            }
            if (pop)
            {
                if (data.Enumerator.Count > 1)
                    data.Enumerator.Pop();
                else
                {
                    goNext = false;
                }

            }
            if (!goNext)
            {
             
                data.Stop(CorFinishState.NaturallyStopped);
            }
            else
            {
                data.Current = en.Current;
               
            }
        }

        CorRaw IInternalController.InternalRegister(CyberCoroutine coroutine, IEnumerator<IWaitable> enumerator,
            Object limit)
        {
            var raw = new CorRaw(coroutine) {Limit = limit};
            raw.InnerStates |= (CorRaw.InnerStateFlags.HasLimit & (CorRaw.InnerStateFlags)(limit != null?1:0));
            raw.Enumerator.Push(enumerator);
            birthList.AddLast(coroutine);
            return raw;
        }
    }

    
}