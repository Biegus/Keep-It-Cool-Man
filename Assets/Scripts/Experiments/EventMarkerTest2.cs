using System;
using UnityEngine;
namespace Experiments
{
    public class EventMarkerTest2 : MonoBehaviour
    {
        private void Update()
        {
            TestEvent.Ev.Call(EventArgs.Empty);
        }
    }
}