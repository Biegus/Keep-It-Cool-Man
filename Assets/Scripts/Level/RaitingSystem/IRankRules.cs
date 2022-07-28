using Cyberultimate;
using UnityEngine;
namespace LetterBattle
{
    public interface IRatingRules 
    {
        RankLevel Rate(float hp, float perfect);
    }
    public class RatingRulesDummy : IRatingRules
    {
        public readonly static RatingRulesDummy Instance = new RatingRulesDummy(); 
        public RankLevel Rate(float hp, float perfect)
        {
            return RankLevel.Unknown;
        }
    }
}