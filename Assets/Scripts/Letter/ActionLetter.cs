using Cyberultimate;
using DG.Tweening;
using System;
using System.Collections.Generic;
using Cyberultimate.Unity;
using LetterBattle.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace LetterBattle
{
	
	
	public class ActionLetter : Spawnable
	{
	
		public enum DeathType
		{
			Unknown,
			KilledByPlayer,
			KilledOnPlayer,
		}

		private readonly static LinkedList<ActionLetter> aliveOnes = new LinkedList<ActionLetter>();
		public static IReadOnlyCollection<ActionLetter> AliveOnes => aliveOnes;

		[SerializeField] private TextMeshProUGUI textEntity;
		
		public char Letter { get; private set; }
		
		public Planet CurrentAvailablePlanet { get; set; }
		public QueueBool LockGettingDmg { get; } = new QueueBool();

		public DeathType DeathReason => deathType;
		public IReadOnlyList<Graphic> Renders => renders;
		public LockValue<uint> Hp = new LockValue<uint>(int.MaxValue,0,1);
		public event EventHandler<ActionLetter> OnEnteredZone = delegate { };
		public event EventHandler<Collider2D> OnTriggerEnter = delegate { };
		
		private Collider2D currentZone;
		private DeathType deathType = DeathType.Unknown;
		private LinkedListNode<ActionLetter> node;
		private Graphic[] renders;
		protected override void Awake()
		{
			base.Awake();
			renders = this.GetComponentsInChildren<Graphic>();
			Hp.OnValueChanged += OnValueChanged;
			Hp.OnValueTaken += (s, e) => LEvents.Base.OnLetterHpLost.Call(this);
			Letter = textEntity.text[0];
			RefreshText();
		}

		private void RefreshText()
		{
			if (Hp.Value == 1)
				textEntity.text = Letter.ToString();
			else
				textEntity.text = $"{new string(Letter,(int)Hp.Value)}";
		}

		private void OnValueChanged(object sender, LockValue<uint>.AnyValueChangedArgs e)
		{
			if (e.Actual == 0) this.Kill(DeathType.KilledByPlayer);
			
			RefreshText();
		}

		protected virtual void Start()
		{
			LevelManager.Current.Hp.OnValueChangedToMin += OnPlayerDeath;
			node= aliveOnes.AddLast(this);
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
			aliveOnes.Remove(node);
			if(LevelManager.Current!=null)
				LevelManager.Current.Hp.OnValueChangedToMin -= OnPlayerDeath;
			LEvents.Base.OnLetterInputed.Raw -= OnLetterPressedInZone;
		}
		private void OnPlayerDeath(object sender, LockValue<float>.AnyValueChangedArgs e)
		{
			this.Kill();
		}

		public void SetLetter(char letter)
		{
			letter = char.ToUpper(letter);
			textEntity.text = letter.ToString();
			Letter = letter;
			RefreshText();
		}
		
		private void OnLetterPressedInZone(object sender, InputLetterArgs args)
		{
			if (!LockGettingDmg &&args.Letter == Letter)
			{
				args.Ask(this, (s,e) =>
				{
					Hp.TakeValue(1);
				});
			}
		}
		public float GetSqrDistanceToClosestPlanet()
		{
			if (CurrentAvailablePlanet == null)
				return float.PositiveInfinity;
			else return (CurrentAvailablePlanet.transform.Get2DPos() - this.transform.Get2DPos()).sqrMagnitude;
		}
		protected override void OnTriggerEnter2D(Collider2D other)
		{
			base.OnTriggerEnter2D(other);
			
			OnTriggerEnter.Invoke(this,other);
			
			if (other.CompareTag("Base"))
			{
				LevelManager.Current.Hp.TakeValue(1, "Base dmged");
			
				Kill( DeathType.KilledOnPlayer);
			}

		}

		public override bool Kill()
		{
			if (base.Kill())
			{
				LEvents.Base.OnLetterDestroyed.Call(this);
				if(deathType != DeathType.Unknown)
					LEvents.Base.OnLetterDestroyedByPlayerInteraction.Call(new LetterActionArgsOnDeath(this, CurrentAvailablePlanet, deathType));
				
				return true;
			}
			return false;

		}
		public  bool Kill(DeathType type)
		{
			if (IsRegisteredForDeath) return false;
			this.deathType = type;
			return Kill();
		}
		

		protected override void OnEnterZone(Collider2D other)
		{

			base.OnEnterZone(other);
			
			CurrentAvailablePlanet = other.gameObject.transform?.GetComponent<Zone>()?.Planet;
			LEvents.Base.OnLetterEnteredZone.Call(this);
			OnEnteredZone(this,this);
			if (CurrentAvailablePlanet == null) throw new InvalidOperationException("Zone was not child of a planet");
			LEvents.Base.OnLetterInputed.Raw -= OnLetterPressedInZone; //for intersections
			LEvents.Base.OnLetterInputed.Raw += OnLetterPressedInZone;
			currentZone = other;
		}
		protected override void OnExitZone(Collider2D other)
		{

			base.OnExitZone(other);
			if(!this.IsRegisteredForDeath)
				LEvents.Base.OnLetterExitedZoneAlive.Call(this);
			if (currentZone == other)
				LEvents.Base.OnLetterInputed.Raw -= OnLetterPressedInZone;
		}
		
	}
}