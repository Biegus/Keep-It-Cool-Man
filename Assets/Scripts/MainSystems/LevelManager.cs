using Cyberultimate;
using DG.Tweening;
using LetterBattle.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cyberultimate.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace LetterBattle
{
	public sealed class LevelManager : Cyberultimate.Unity.MonoSingleton<LevelManager>
	{
		private const string TARGET_NOT_FOUND = "Target not found";
		public enum Status
		{
			Unknown = 0,

			Spawning = 1,
			///<summary> Player can still die while being in that state, level will end when nothing blocks SceneBlocker </summary>
			WaitingForWin = 2,
			///<summary> Once encounter, this state is unchangeable, level will end when nothing blocks SceneBlocker</summary>
			WaitingForGameOver = 3,

		}
		[SerializeField]
		private AudioSource source = null;
		[SerializeField]
		private AudioSource comboSource = null;
		[SerializeField]
		private float gameOverDelay = 3;
		[SerializeField]
		private float winDelay = 5;
		[SerializeField] private Transform parent;
		[SerializeField]
		private SReadonlyArray<Transform> lines = new SReadonlyArray<Transform>();
		[SerializeField]
		private SReadonlyArray<StraightDirection> directionsOrder = new SReadonlyArray<StraightDirection>();
		
		public IReadOnlyList<Planet> Bases { get; private set; } = null;
	
		public LockValue<float> Hp { get; private set; }
		public QueueBool SceneBlockerStatus { get; private set; } = new QueueBool();
		public SReadonlyArray<StraightDirection> DirectionsOrder => directionsOrder;
		public Status LevelStatus { get; private set; } = Status.Spawning;
		public CanvasGroup HPUI { get; private set; } // it should not have this reference tho
		public int CurrentPhaseIndex { get; private set; }= 0;
	

	
		public List<Transform> Targets { get; } = new List<Transform>();
		public IReadOnlyList<Transform> Lines => lines.Array;
		public Transform Parent => parent;
		
		
		[NaughtyAttributes.ShowNativeProperty]
		private float EditorHp => Hp?.Value ?? -1;
		private Tween endTween = null;
		private Cooldown startCooldown;
		private float levelStartTime = 0;
		
		protected override void Awake()
		{
			base.Awake();

#if UNITY_EDITOR
			if (GameManager.CurrentLevel == null)
			{
				
				SceneManager.LoadScene("Menu");
				return;
			}
#endif
			
			Hp = new LockValue<float>(GameManager.CurrentLevel.StartHp, 0, GameManager.CurrentLevel.StartHp, float.MaxValue);
			Hp.OnValueChanged += OnHpChanged;
			Hp.OnValueChangedToMin += OnDeath;
			LEvents.Base.OnTargetNotFound.Raw += OnTargetNotFound;
			LEvents.Base.OnLetterEnteredZone.Raw += OnLetterEnteredZone;
			LEvents.Base.OnComboChanged.Raw += OnComboChanged_Raw;
			GetPhase().Init(null);
		}
	
		private void OnComboChanged_Raw(object sender, LEvents.ComboChangeArgs e)
		{
			comboSource.pitch = (float)-Math.Log(e.CurrentCombo, 1500) + 1;
			if (e.CurrentCombo > e.PreviousCombo &&
			e.CurrentCombo == Mathf.Floor(e.CurrentCombo))
			{
				comboSource.PlayOneShot(GameAsset.Current.ComboSound);
			}

		}

		private void OnLetterEnteredZone(object sender, ActionLetter args)
		{
			
			source.PlayOneShot(GameAsset.Current.ZoneEnterSound);
		}
		private void OnHpChanged(object sender, LockValue<float>.AnyValueChangedArgs args)
		{
			if (args.Action == LockValue<float>.Action.Take && (string)args.From != "time")
			{
				CameraHelper.Current.ShakeScreen(25, 0.1f);
				if (((string)args.From) != TARGET_NOT_FOUND)
				{
					source.PlayOneShot(GameAsset.Current.DmgSound);
				}
			}
		}
		public void RegisterAsTarget(Transform target, int order)
		{

			if (Targets.Count <= order)
			{
				Targets.AddRange(Enumerable.Range(0, order - Targets.Count + 1).Select(item => (Transform)null));

			}
			else if (Targets[order] != null)
			{
				Debug.LogWarning($"Target {order} was assigned twice");
			}
			Targets[order] = target;

		}
		
		private IEnumerator Start()
		{
			startCooldown = new Cooldown(1.4f);
			if (GameManager.CurrentLevel.StartText != string.Empty)
			{
				InfoBoxSpawner.Current.Spawn(new Vector2(-1,-1), GameManager.CurrentLevel.StartText,GameAsset.Current.Pallete.GetColor(ColorType.Danger),10f);
				
			}
			Bases = FindObjectsOfType<Planet>();
			HPUI = Bases[0].HpUICanvas;
			
			if (GameManager.CurrentLevel.AdditionalObjectsToSpawn != null)
				foreach (var element in GameManager.CurrentLevel.AdditionalObjectsToSpawn)
				{
					Instantiate(element);
				}
			GameManager.MusicSource.UnPause();

			LEvents.Base.OnLevelStarted.Call(EventArgs.Empty);

			yield return null;
			if (this.Targets.Count == 0)
			{
				Debug.LogError($"{nameof(LevelManager)}: No targets were registered, drop {nameof(TargetRegisterer)} on obj that should be target withing scene with planets");
			}
		}
		private void OnDestroy()
		{
			LEvents.Base.OnTargetNotFound.Raw -= OnTargetNotFound;
			LEvents.Base.OnComboChanged.Raw -= OnComboChanged_Raw;
			this.SceneBlockerStatus.Event -= NoneLetterBlocker;
			if (InputReader.Current != null) InputReader.Current.DeafStatus.Unregister(this);
			LEvents.Base.OnLetterEnteredZone.Raw -= OnLetterEnteredZone;

		}


		private void OnTargetNotFound(object sender, char args)
		{

			InfoBoxSpawner.Current.Spawn(null, "NOT FOUND", GameAsset.Current.Pallete.GetColor(ColorType.Danger));
			if (GameManager.CurrentLevel.PunishForWrong)
			{
				Hp.TakeValue(GameAsset.Current.DmgForWrong, TARGET_NOT_FOUND);

			}
			source.PlayOneShot(GameAsset.Current.WrongLetterSound);

		}
		private void OnDeath(object sender, EventArgs args)
		{
			if (LevelStatus == Status.WaitingForGameOver) return;
			endTween?.Kill(false);// if the level win was already scheduled, it will be stopped
			
			source.PlayOneShot(GameAsset.Current.DeathSound);
			endTween = EndSceneWhenPossible(gameOverDelay, Status.WaitingForGameOver);

			GameManager.MusicSource.Pause();

		}
		private Tween EndSceneWhenPossible(float minimalTime, Status status)
		{

			if (status is Status.Spawning or Status.Unknown) throw new InvalidOperationException($"{status} is not correct status for this context");
			if (LevelStatus == Status.WaitingForGameOver) throw new InvalidOperationException("State of WaitingForGameOver cannot be changed");
			if (LevelStatus == Status.WaitingForWin && status == Status.WaitingForWin) throw new InvalidOperationException("Level is already waiting for win");
			if (status == Status.WaitingForGameOver)
			{
				InputReader.Current.DeafStatus.RegisterObj(this);
			}
			LevelStatus = status;
			float startTime = Time.time;
			Tween tween = null;
			tween = DOVirtual.Float(0, 1, float.PositiveInfinity, _ =>
			{
				if (Time.time - startTime < minimalTime) return;
				bool isBlocked = SceneBlockerStatus.Evaluate();
				if (Time.time - levelStartTime > GameManager.CurrentLevel.Time + 13)
				{
					ActionLetter.AliveOnes
						.Foreach(item=>item.Kill());
					Debug.Log("Level didn't finish on it's own, finish was forced");

					isBlocked = false;
				}

				if (isBlocked) return;
				tween.Kill(false); /* i can't just use onComplete cause it' does it twice while invoking from inside*/
				if (this.gameObject == null) return;
				InputReader.Current.DeafStatus.RegisterObj(this);

				if (status == Status.WaitingForWin)
				{
					GameManager.SetScoreForLevel(GameManager.CurrentLevelNumber,ScoreSystem.Current.GenerateScoreData());
					if (Hp.Value <=1f)
					{
						SteamHelper.Unlock("BEAT_ACHIEVEMENT_2");//beat level with maximum temperature
					}
					LEvents.Base.OnLevelWon.Call(EventArgs.Empty);				
				}

				else
				{
						
					LEvents.Base.OnLevelLost.Call(EventArgs.Empty);
				}

			}).SetLink(this.gameObject);
			endTween = tween;
			return tween;
		}

		private void OnDrawGizmos()
		{

			if (lines.Array.Count == 0)
			{
				return;
			}
			Vector2 start = lines.Array[0].transform.position;
			foreach (var item in lines.Array.Skip(1))
			{

				Gizmos.DrawLine(start, item.position);
				start = item.position;
			}
			Gizmos.DrawLine(start, lines.Array[0].transform.position);
		}
		
		private void Update()
		{
			
			if (!startCooldown.Peek()) return;
			if (LevelStatus != Status.Spawning) return;
			if (!GetPhase()?.Update()??false )
			{
				if (CurrentPhaseIndex >= GameManager.CurrentLevel.Phases.Count - 1)
				{
					this.SceneBlockerStatus.Event += NoneLetterBlocker;
					EndSceneWhenPossible(winDelay, status: Status.WaitingForWin);

					return;
				}
				CurrentPhaseIndex++;
				GetPhase().Init(GameManager.CurrentLevel.Phases[CurrentPhaseIndex-1]);
			}

		}

		public Phase GetPhase()
		{
			if (GameManager.CurrentLevel == null)
				return null;
			if (CurrentPhaseIndex >= GameManager.CurrentLevel.Phases.Count)
				return null;
			return GameManager.CurrentLevel.Phases[CurrentPhaseIndex];
		}
		private void NoneLetterBlocker(object sender, BoolResolverArgs args)
		{
			args.SendSignal(ActionLetter.Amount != 0);
		}



	}
}