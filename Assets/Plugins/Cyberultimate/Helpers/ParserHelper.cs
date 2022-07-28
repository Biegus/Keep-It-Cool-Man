#nullable enable
#if UNITY_EDITOR || UNITY_STANDALONE ||UNITY_WII|| UNITY_IOS || UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_PS4 || UNITY_XBOXONE || UNITY_LUMIN || UNITY_TIZEN ||UNITY_TVOS || UNITY_WSA || UNITY_WSA_10_0 || UNITY_WINRT || UNITY_WINRT_10_0 ||UNITY_WEBGL ||UNITY_FACEBOOK||UNITY_FACEBOOK||UNITY_ADS || UNITY_ANALYTICS ||UNITY_ASSERTIONS ||UNITY_64
#define ANY_UNITY
#endif
using System;
using System.Collections.Generic;
using Cyberultimate.Unity;
using UnityEngine;

namespace Cyberultimate
{
    public class ParserHelper
    {
        private static readonly Dictionary<Type, Func<string, object>> Parsers =
            new Dictionary<Type, Func<string, object>>()
            {
                [typeof(byte)] = (v) => byte.Parse(v),
                [typeof(sbyte)] = (v) => sbyte.Parse(v),
                [typeof(short)] = (v) => short.Parse(v),
                [typeof(ushort)] = (v) => ushort.Parse(v),
                [typeof(int)] = (v) => int.Parse(v),
                [typeof(uint)] = (v) => uint.Parse(v),
                [typeof(long)] = (v) => long.Parse(v),
                [typeof(ulong)] = (v) => long.Parse(v),
                [typeof(float)] = (v) => float.Parse(v),
                [typeof(double)] = (v) => double.Parse(v),
                [typeof(decimal)] = (v) => decimal.Parse(v),
                [typeof(string)]=(v)=>v,
                [typeof(bool)]=(v)=>BoolHelper.BetterParse(v),
#if ANY_UNITY
                [typeof(Vector2)]=(v)=>Cyberultimate.Unity.Vector2Helper.Parse(v),
                [typeof(Vector2Int)]=(v)=> Vector2Helper.Parse(v).ToVector2Int(),
                
#endif
                

            };

        public static object Parse(string text,Type type)
        {
            return Parsers[type](text);
        }
        
    }
}