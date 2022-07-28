#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Cyberultimate.Unity
{
    public class TimeLockerArgs : EventArgs
    {
        public float Modifier { get; }
        public object Locker { get; }
        public bool RemovedAction { get; }
        public TimeLockerArgs(float modifier, object locker, bool removedAction)
        {
            Modifier = modifier;
            Locker = locker;
            RemovedAction = removedAction;
        }
    }
    [Obsolete("Use TimeScalling")]
    /// <summary>
    /// Better version to manipulate time scale than <see cref="Time.timeScale"/>. 
    /// </summary>
    public static class TimeControl
    {
        public static ReadOnlyDictionary<object, float> Lockers => lazyLockers.Value;
        public static float TimeScale => Time.timeScale;
        public static event EventHandler<SimpleArgs<float>> OnTimeScaleChanged = delegate { };
        public static event EventHandler<TimeLockerArgs> OnNewLockerAdded = delegate { };
        public static event EventHandler<TimeLockerArgs> OnAnyLockerRemove = delegate { };
        public static event EventHandler<TimeLockerArgs> OnAnyLockerChanged = delegate { };

        private static Lazy<ReadOnlyDictionary<object, float>> lazyLockers = new Lazy<ReadOnlyDictionary<object, float>>
            (() => new ReadOnlyDictionary<object, float>(scalers));
        private static readonly Dictionary<object, float> scalers = new Dictionary<object, float>();
        private static bool refreshLock = false;
     
#if UNITY_EDITOR
        private static float lastRemembered = 1;
#endif
        
#if UNITY_EDITOR
        private static object fastLocker = new object();
        [MenuItem("Cyberultimate/TimeControll/ShowLockers")]
        private static void ShowLockers()
        {
            Debug.Log(
                scalers.Select(item=>item.Value).ToDebugString()
            );
        }
        [MenuItem("Cyberultimate/TimeControll/FreezeTime")]
        private static void FreezeTime()
        {
            Register(fastLocker,0);
        }
        [MenuItem("Cyberultimate/TimeControll/UnFreezeTime")]
        private static void UnFreezeTime()
        {
            Unregister(fastLocker);
        }

#endif

        /// <summary>
        /// If locker isn't already in, adds multiplying value. 
        /// If locker is in, changes the locker value. 
        /// Make sure you will be able to remove locker when you have done with it.
        public static void Register(object locker, float value)
        {
            if (locker == null)
                throw new ArgumentNullException(nameof(locker));
            if (scalers.ContainsKey(locker))
            {
                scalers[locker] = value;
                OnAnyLockerChanged(null, new TimeLockerArgs(value, locker, false));
            }
            else
            {
                scalers.Add(locker, value);
                OnNewLockerAdded(null, new TimeLockerArgs(value, locker, false));
            }
            
            Refresh();
        }
        /// <summary>
        /// Removes multiplying value. It should be always done before locker get lost 
        /// </summary>
        /// <param name="locker"></param>
        public static bool Unregister(object locker)
        {
            if (!scalers.TryGetValue(locker, out var val)) return false;
            OnAnyLockerRemove(null, new TimeLockerArgs(val, locker, true));
            scalers.Remove(locker);
            Refresh();
            return true;

        }
        /// <summary>
        /// Clears all lockers.
        /// </summary>
        /// <param name="doLockerEvents"></param>
        public static void ClearAll(bool doLockerEvents)
        {
            if (doLockerEvents)
            {
                refreshLock = true;
                foreach (var item in scalers.Keys)
                {
                    Unregister(item);
                }
                refreshLock = false;
            }
            else
                scalers.Clear();
            Refresh();

          
        }
        private static void Refresh()
        {
            if (refreshLock)
                return;
#if UNITY_EDITOR
            if (Time.timeScale != lastRemembered)
            {
                Debug.LogWarning("If you use TimeControl, you shouldn't change Time.timeScale manually");
            }
#endif
            float scale = scalers.Aggregate(1f,(a,b) => a*b.Value);
            if (scale != Time.timeScale)
            {
                Time.timeScale = scale;
                OnTimeScaleChanged(null, scale);
#if UNITY_EDITOR
                lastRemembered = scale;
#endif
            }
        }


    }
}

