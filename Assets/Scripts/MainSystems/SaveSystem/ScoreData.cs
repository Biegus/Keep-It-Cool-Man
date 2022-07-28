namespace LetterBattle
{
    public struct ScoreData
    {
        public static  readonly ScoreData None;
        private int _Number;
        public float Combo { get; set; }
        public RankLevel Rank { get; set; }

        public int Number
        {
            get => _Number - 1;
            set => _Number = value + 1;
        }
        
        public ScoreData(int number, float bestCombo,RankLevel rank)
        {
            _Number = 0;
            Combo = bestCombo;
            Rank = rank;
            Number = number;
           

        }
        public static ScoreData GetBetter(in ScoreData a, in ScoreData b)
        {
            if (a.Rank == RankLevel.Unknown && a != b) return b;
            if (b.Rank == RankLevel.Unknown && a != b) return a;
            if (a.Rank == b.Rank)
                return a.Number > b.Number ? a : b;
            else return a.Rank < b.Rank ? a : b;

        }
        public static bool operator ==(in ScoreData a, in ScoreData b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(in ScoreData a, in ScoreData b)
        {
            return !a.Equals(b);
        }
        public override string ToString()
        {
            return $"(Value={Number}, Combo=x{Combo})";
        }

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}