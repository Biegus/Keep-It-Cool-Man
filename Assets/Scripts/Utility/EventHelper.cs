using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace LetterBattle
{
    public class EventHelper
    {
        public static void InvokeEventSafe(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                Debug.LogError($"Exception during  callable event \n Exception vvv");
                Debug.LogException(exception);
            }
        }
    }
}