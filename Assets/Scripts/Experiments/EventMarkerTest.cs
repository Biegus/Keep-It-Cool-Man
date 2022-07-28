using System;
using LetterBattle;
using UnityEngine;
namespace Experiments
{
    public static class TestEvent
    {
        public readonly static CallableEvent<EventArgs> Ev = new CallableEvent<EventArgs>();
    }
    public class EventMarkerTest : MonoBehaviour
    {
        private void Awake()
        {
            CallableEventMarker.DoInStartSpace(this, () =>
            {
                TestEvent.Ev.Register((s,v)=>{ Debug.Log("HI");});
               
            });
           
        }
        private void OnDestroy()
        {
            CallableEventMarker.End(this);
        }
    }
}