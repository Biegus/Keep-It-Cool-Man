using System;
using System.Collections.Generic;
using System.Linq;
using Cyberultimate.Unity;
using DG.Tweening;
using UnityEngine;
namespace LetterBattle.Utility
{
    public static class CircleSettingHelper
    {
        [Serializable]
        public struct SettingData
        {
            public float Difference;
            public float Radius;

            public SettingData(float difference, float radius)
            {
                Difference = difference;
                Radius = radius;
            }
        }
        public static (Vector2 pos, float angle) GetPosAndAngle(in  SettingData data, float v, float modf=0)
        {
            float angleValue = ( 1-(2 * v)) * data.Difference;
            float raw = Mathf.PI/2 + angleValue;
              
            float x = Mathf.Cos(raw) * data.Radius;
            float y = Mathf.Sin(raw) * data.Radius;
            return (new Vector2(x, y).GetRotated(modf), (angleValue)%360);
        }
       
    }
}