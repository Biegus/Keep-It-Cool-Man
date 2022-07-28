using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cyberultimate;
using Cyberultimate.Unity;
using NaughtyAttributes;
namespace LetterBattle
{
    [Serializable]
    public struct RankRulesElement
    {
        public int MinHp;
        [Range(0,1)]
        public float MinPerfect;
    }
   
    [Serializable]
    public class RankRules : IRatingRules
    {

       [SerializeField][CurveRange(0,0,3,1,EColor.Orange)]
        private AnimationCurve hpMin = new AnimationCurve();
        [SerializeField][CurveRange(0,0,3,1,EColor.Orange)]
        private AnimationCurve perfectMin = new AnimationCurve();

        public float EvaluateHp(int index)
        {
            return hpMin.Evaluate(index);
        }
        public float EvaluatePerfect(int index)
        {
            return perfectMin.Evaluate(index);
        }
        public RankRules(){}
        
        private float GetHp(int index)
        {
            return hpMin.Evaluate(index);
        }
        private float GetPefect(int index)
        {
            return perfectMin.Evaluate(index);
        }
        //naturally created object will have them correct for sure
        public bool ValidateRules()
        {
            for (int i = 0; i < 4; i++)
            {
                if (GetHp(i-1) < GetHp(i) && GetPefect(i-1)< GetPefect(i))
                    return false;
            }
            return true;
        }
        
        public RankLevel Rate(float hpPercentage, float perfect)
        {
            for (var index = 0; index < 4; index++)
            {
                if (hpPercentage >= GetHp(index) && perfect>=GetPefect(index))
                {
                    return (RankLevel)(index + 1);
                }
            }
            return RankLevel.D;
        }
        
    }
}