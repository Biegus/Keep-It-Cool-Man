using Cyberultimate;
using Cyberultimate.Unity;
using LetterBattle.Utility;
using System;
using UnityEngine;
namespace LetterBattle
{

	public class ScoreSystem : MonoSingleton<ScoreSystem>
	{
		private float timeSinceCombo = 0;

		public float ResetTime;
		public LockValue<int> Score { get; private set; }

		public int AmountInRow { get; private set; } = 1;
		public float Combo { get; private set; } = 1;
		private float previousCombo = 1;

		private bool levelEnded;

		private int hitsQuantity=0;
		private int elementsInQuantity=0;
		public RankLevel GetRank() => GameManager.CurrentLevel.RankRules.Rate(LevelManager.Current.Hp.Value/LevelManager.Current.Hp.Max, PerfectPercent);
		public float PerfectPercent => ((float)(hitsQuantity)) / elementsInQuantity;
		public ScoreData GenerateScoreData()
		{
			return new ScoreData(Score.Value, Combo,GetRank());
		}

		protected override void Awake()
		{
			base.Awake();
			Score = new LockValue<int>(int.MaxValue, 0, 0);
			Score.OnValueChanged += OnScoreChanged;

			LEvents.Base.OnLevelLost.Raw += OnLevelFinished;
			LEvents.Base.OnLevelWon.Raw += OnLevelFinished;
			LEvents.Base.OnLetterDestroyedByPlayerInteraction.Raw += OnHit;
			LEvents.Base.OnLetterEnteredZone.Raw += OnLetterEnteredZone;
			LEvents.Base.OnLetterExitedZoneAlive.Raw += OnLetterExitedZoneAlive;
		


		}
		private void OnLetterExitedZoneAlive(object sender, ActionLetter e)
		{
			elementsInQuantity--;
		}
		private void OnLetterEnteredZone(object sender, ActionLetter e)
		{
			elementsInQuantity++;
		}
		private void OnHit(object sender, LetterActionArgsOnDeath e)
		{
			if (e.DeathType == ActionLetter.DeathType.KilledByPlayer)
				hitsQuantity++;
		}
		
		private void OnScoreChanged(object sender, LockValue<int>.AnyValueChangedArgs e)
		{
			if (e.Action == LockValue<int>.Action.Add)
				AmountInRow++;
			else
				AmountInRow = 1;
			RefreshCombo();

		}
		public void AddScoreWithComboRespect(int score, object? from)
		{
			if (score > 0)
				Score.GiveValue(Mathf.RoundToInt(score * Combo), from);
			else
				Score.GiveValue(score, from);
		}
		private void RefreshCombo()
		{
			if (levelEnded)
			{
				Combo = previousCombo;
				return;
			}

			Combo = (float)Math.Round(Mathf.Sqrt(AmountInRow), 2);

			LEvents.Base.OnComboChanged.Call(new LEvents.ComboChangeArgs(Combo, previousCombo));

			timeSinceCombo = Time.time;
			previousCombo = Combo;
		}

		private void Update()
		{
			if (levelEnded)
			{
				return;
			}

			if (Time.time - timeSinceCombo >= ResetTime)
			{
				AmountInRow = 1;
				RefreshCombo();
			}
		}
		private void OnLevelFinished(object sender, EventArgs args)
		{
			levelEnded = true;
		}

		private void OnDestroy()
		{
			LEvents.Base.OnLevelLost.Raw -= OnLevelFinished;
			LEvents.Base.OnLevelWon.Raw -= OnLevelFinished;
			LEvents.Base.OnLetterDestroyedByPlayerInteraction.Raw -= OnHit;
		}
	}
}