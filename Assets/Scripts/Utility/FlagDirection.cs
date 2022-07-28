using System;
using System.Collections.Generic;
using Cyberultimate;
using Cyberultimate.Unity;
using UnityEngine;
namespace LetterBattle.Utility
{
    [Serializable]
    [Flags]
    public enum FlagDirection
    {
        Up=SimpleDirection.Up,
        Down=SimpleDirection.Down,
        Left=SimpleDirection.Left,
        Right=SimpleDirection.Right,
        LeftUp = 1<<4,
        LeftDown = 1<<5,
        RightUp = 1<<6,
        RightDown = 1<<7,
    }
    
    
    public static class FlagDirectionExtension
    {
        public static int CountSides(this FlagDirection direction)
        {
            int intRepresentation = (int)direction;
            
            int count = 0;
            while (intRepresentation > 0) 
            {
                count += intRepresentation & 1;
                intRepresentation >>= 1;
            }
            return count;
        }

        public static IEnumerable<FlagDirection> ToEnumerable(this FlagDirection direction)
        {
            int intRepresentation = (int)direction;
            
            int count = 0;
            while (intRepresentation > 0)
            {
                if ((intRepresentation & 1) == 1)
                {
                    yield return (FlagDirection)(1 << count);
                }

                intRepresentation >>= 1;
                count++;

                if (count >= 8)
                    break;
            }
            
        }
        
        public static Direction ToDirection(this FlagDirection direction)
        {
            int val =(int)  direction;
            
            if (val <= 1 << 3)
            {
                return ((SimpleDirection)(direction)).ToDirection();
            }
            return direction switch
            {
                FlagDirection.LeftUp => Direction.LeftUp,
                FlagDirection.LeftDown => Direction.LeftDown,
                FlagDirection.RightUp => Direction.RightUp,
                FlagDirection.RightDown => Direction.RightDown,
                _=> throw new ArgumentException($"{direction} direction is incorrect")
            };
        }
    }
    [Serializable]
   

    public enum StraightDirection
    {
        Up=SimpleDirection.Up,
        Down=SimpleDirection.Down,
        Left=SimpleDirection.Left,
        Right=SimpleDirection.Right
    }
}