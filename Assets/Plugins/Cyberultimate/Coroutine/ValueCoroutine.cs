#nullable enable
using System;
using System.Collections.Generic;
using Cyberultimate.Unity;
using Object = UnityEngine.Object;

namespace Cyberultimate.Unity
{
    namespace  Internal
    {
        public interface IValueCoroutineInternal
        {
            void InternalSetResult(object? value);
        }
    }
    public class ValueCoroutine<T> : CyberCoroutine, Internal.IValueCoroutineInternal
    {
        public T Result { get; private set; } = default!;//generic issue before c# 9.0
        public bool HasResult { get; private set; } 
        public ValueCoroutine(IEnumerator<IWaitable> enumerator, CorController controller, Object limit)
            : base(enumerator, controller, limit)
        {
        }
     
       
        public ValueCoroutine<T> OnEndWithValue(CorFinishState required,Action<ValueCoroutine<T>> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            this.OnFinished += (s, e) =>
            {
                if (required == e.WayOfStopping)
                    action((s as ValueCoroutine<T>)!);
            };
            return this;
        }

        void Internal.IValueCoroutineInternal.InternalSetResult(object? value)
        {
            Result = (T)value;
        }
    }
}