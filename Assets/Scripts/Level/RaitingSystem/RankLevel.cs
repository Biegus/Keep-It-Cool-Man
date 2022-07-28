namespace LetterBattle
{
    public enum RankLevel
    {
        Unknown=0,
        APlus=1,
        A=2,
        B=3,
        C=4,
        D=5
    }
    public static class RankLevelExtension
    {
        public static RankTextData GetTextData(this RankLevel rank)
        {
            return GameAsset.Current.TextsForRanks[(int)rank];
        }
    }
}