using System;
using LetterBattle.Utility;
using UnityEngine;
namespace LetterBattle
{
    //down time scaling is prioritized
    public static class TimeScaling
    {
        
        public static QueueValue<float> Status = new(1, Math.Min);
         static TimeScaling()
         {
             Status.OnValueChanged += OnValueChanged;
         }
         private static  void OnValueChanged(object sender, QueueValue<float> queue)
         {
             Time.timeScale = queue.Value;
         }
    }
}