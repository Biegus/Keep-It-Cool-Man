#nullable enable
using System;
using Cyberultimate.Unity.Internal;
using UnityEngine;

namespace Cyberultimate.Unity
{
    
    public static class Yield
    {

      
        public static IWaitable? NextFrame { get; } = null;
       
        
        public static IWaitable Wait(float span, bool ignoreTimeScale=false)
        {
            return new WaitTime(span, ignoreTimeScale);
        }
        public static IWaitable Until(Func<bool> predict)
        {
            return new CondictionalWait(predict);
        }
        public static IWaitable ToLoop(CorLoopState loop)
        {
            return new StateChange(loop);
        }

        public static IWaitable Return(object val)
        {
            return new CorReturn(val);
        }
        public static IWaitable Frames(int amount)
        {
            return new WaitFrames(amount);
        }

        public static IWaitable EscapeThread(bool useThreadPool=true)
        {
            return new EscapeThread(useThreadPool);
        }

        public static IWaitable WaitForEvent<T>(Action<EventHandler<T>> add, Action<EventHandler<T>> remove)
        {
            if (add is null)
                throw new ArgumentNullException(nameof(add));
            if (remove is null)
                throw new ArgumentNullException(nameof(remove));
            bool isReady = false;
            bool Predicts() => isReady;

            add(Waiting);
            void Waiting(object sender, T args)
            {
                isReady = true;
                remove(Waiting);
            }

            return new CondictionalWait(Predicts);
        }
        
        private class CondictionalWait : IWaitableCondictional
        {
            private Func<bool> predict;

            public CondictionalWait(Func<bool> predict)
            {
                this.predict = predict ?? throw new ArgumentNullException(nameof(predict));
            }
            public bool IsReady()
            {
                return predict();
            }

            public void Pause()
            {
            }

            public void Resume()
            {
            }
        }
        
        
        //waitframes and wait time are almost identical.
        //mutual base class would be nice but c# generics with numbers are broken.
        //it would cost more to make it work with int as well as floats, than just having
        //that small repetition.

        private class WaitFrames : IWaitableCondictional
        {
            private int amount = 0;
            private int start = 0;

            public WaitFrames(int amount)
            {
                if (amount < 0)
                    throw new ArgumentException("Amount cannot be negative");
                this.amount = amount;
                start = Time.frameCount;
            }
            public bool IsReady()
            {
                return Time.frameCount - start >= amount;
            }

            public void Pause()
            {
                amount -= (Time.frameCount - start);
            }

            public void Resume()
            {
                start = Time.frameCount;
            }
        }
        
        private class WaitTime : IWaitableCondictional
        {
            private float timeAtStart;
            private float span;
            private bool ignoreTimeScale;
            private bool paused = false;
            private float GetTime()
            {
                return ignoreTimeScale ? Time.unscaledTime : Time.time;
            }
            public WaitTime(float span,bool ignoreTimeScale)
            {
                if (span < 0)
                    throw new ArgumentException("Span cannot be negative");
                this.span = span;
                this.ignoreTimeScale = ignoreTimeScale;
                timeAtStart = ignoreTimeScale ? Time.unscaledTime : Time.time;
            }
            public  bool IsReady()
            {
                return
                    (GetTime() - timeAtStart >= span);

            }

            public  void Pause()
            {
                if (paused) return;
                paused = true;
                span -= GetTime() - timeAtStart;// made span smaller so we can pretend it starts when resumes from beginning, only with smaller goal
            }

            public  void Resume()
            {
                if (!paused) return;
                paused = false;
                timeAtStart = GetTime();
            }
        }
       
    }
}