using System;
using Cyberultimate;
using LetterBattle.Utility;
using NaughtyAttributes;
using UnityEngine;
namespace LetterBattle
{
    [Serializable]
    public struct SpawnField
    {
        public StraightDirection Direction;
        [Range(0,1)]
        public float From;
        [Range(0,1)]
        public float To;
        public int TargetIndex;
    }
}