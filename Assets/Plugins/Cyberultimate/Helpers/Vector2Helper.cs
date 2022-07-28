#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.Globalization;

namespace Cyberultimate.Unity
{
    public static class Vector2Helper
    {
        public static Vector2 GetDominating(this Vector2 original)
        {
            Vector2 changer;
            if (Math.Abs(original.x) < Math.Abs(original.y))
            {
                changer = new Vector2(1, 0);
            }
            else
            {
                changer = new Vector2(0, 1);
            }
            return changer;
        }
       
        /// <summary>
        /// Rotating vector.
        /// </summary>
        /// <param name="vect"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 GetRotated(this Vector2 vect, float angle)
        {
            float radianAngle = angle * Mathf.Deg2Rad;
            float x = vect.x * Mathf.Cos(radianAngle) - vect.y * Mathf.Sin(radianAngle);
            float y = vect.x * Mathf.Sin(radianAngle) + vect.y * Mathf.Cos(radianAngle);
            return new Vector2((float)x, (float)y);
        }
        public static Vector2 GetRotated(this Vector2 vect, Quaternion quaternion)
        {
            return GetRotated(vect, quaternion.eulerAngles.z);
        }
        public static Direction ToDirection(this Vector2 vect)
        {
            return new Direction(vect);
        }
        /// <summary>
        /// Parsing from text.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Vector2 Parse(string text)
        {
            text = text.Replace("(", "").Replace(")", "").Replace(" ", "");
            string[] split = text.Split(',');
            return new Vector2(float.Parse(split[0], CultureInfo.InvariantCulture),
                float.Parse(split[1], CultureInfo.InvariantCulture));
        }
        public static Vector2 CX(this Vector2 vector,float x)
        {
            return new Vector2(x,vector.y);
        }
        public static Vector2 CY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, y);
        }

        public static Vector2Int ToVector2Int(this Vector2 v)
        {
            return new Vector2Int((int)v.x, (int)v.y);
        }
        public static float SqrDistance(this Vector2 a, Vector2 b)
        {
            return (a - b).sqrMagnitude;
        }
        
    }
}
