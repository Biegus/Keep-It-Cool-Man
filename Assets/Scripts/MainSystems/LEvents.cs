using System;
using Cyberultimate;
using UnityEngine;
namespace LetterBattle
{
	public class LetterActionArgsOnDeath
	{
		public Planet Planet { get; }
		public ActionLetter ActionLetter { get; }
		public ActionLetter.DeathType DeathType { get; }
		public LetterActionArgsOnDeath(ActionLetter actionLetter, Planet planet, ActionLetter.DeathType deathType)
		{
			this.ActionLetter = actionLetter;
			Planet = planet;
			this.DeathType = deathType;
		}
	}
	
	public sealed  class LEvents
	{
		public static  LEvents Base = new LEvents();

		public class ComboChangeArgs
		{
			public float CurrentCombo { get; }
			public float PreviousCombo { get; }

			public ComboChangeArgs(float comboNow, float comboPrevious)
			{
				CurrentCombo = comboNow;
				PreviousCombo = comboPrevious;
			}
		}
		
		

		public CallableEvent<InputLetterArgs> OnLetterInputed { get; }
		/// <summary>
		/// Success= false means that the letter took player hp
		/// </summary>
		public CallableEvent<LetterActionArgsOnDeath> OnLetterDestroyedByPlayerInteraction { get; } // why is it splitted? related to legacy code.
		public CallableEvent<ActionLetter> OnLetterHpLost { get; }
		public CallableEvent<ActionLetter> OnLetterDestroyed { get; }
		public CallableEvent<char> OnTargetNotFound { get; }
		public CallableEvent<ActionLetter> OnLetterEnteredZone { get; }
		public CallableEvent<ActionLetter> OnLetterExitedZoneAlive { get; }
		public CallableEvent<EventArgs> OnGameEnded { get; }
		public CallableEvent<EventArgs> OnLevelStarted { get; }
		public CallableEvent<ComboChangeArgs> OnComboChanged { get; }
		public CallableEvent<EventArgs> OnLevelLost { get; }
		public CallableEvent<EventArgs> OnLevelWon { get; }
		public CallableEvent<SpawnBehavior> OnSpawnBehaviourUsed { get; }
		
		
		public void Input(char letter)
		{
			var args = new InputLetterArgs(letter);
			LEvents.Base.OnLetterInputed.Call(args);
			if (args.Registered.Count == 0)
				LEvents.Base.OnTargetNotFound.Call(letter);
			else
				args.Registered.MinOriginal(item =>
				{
					return item.obj.GetSqrDistanceToClosestPlanet();
				}).ev(this, letter);
		}
		private LEvents()
		{
			OnLetterExitedZoneAlive = new CallableEvent<ActionLetter>();
			OnLetterInputed = new CallableEvent<InputLetterArgs>();
			OnLetterDestroyedByPlayerInteraction = new CallableEvent<LetterActionArgsOnDeath>();
			OnTargetNotFound = new CallableEvent<char>();
			OnLetterEnteredZone = new CallableEvent<ActionLetter>();
			OnGameEnded = new CallableEvent<EventArgs>();
			OnLevelStarted = new CallableEvent<EventArgs>();
			OnComboChanged = new CallableEvent<ComboChangeArgs>();
			OnLevelLost = new CallableEvent<EventArgs>();
			OnLevelWon = new CallableEvent<EventArgs>();
			OnLetterDestroyed = new CallableEvent<ActionLetter>();
			OnSpawnBehaviourUsed = new CallableEvent<SpawnBehavior>();
			OnLetterHpLost = new CallableEvent<ActionLetter>();

		}

	}
}