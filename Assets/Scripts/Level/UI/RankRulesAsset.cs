using UnityEngine;
namespace LetterBattle
{
    [CreateAssetMenu(menuName = "RankRulesAsset")]
    public class RankRulesAsset :ScriptableObject,  IRatingRules
    {
        [SerializeField]
        private RankRules rules;
        public RankRules Rules => rules;

        public RankLevel Rate(float hp, float perfect)
        {
            return rules?.Rate(hp, perfect)?? RankLevel.Unknown;
        }
    }
}