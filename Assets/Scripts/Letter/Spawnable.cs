using Cyberultimate;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Cyberultimate.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace LetterBattle
{
	[CommandContainer]
	public class Spawnable : MonoBehaviour
	{	
		[SerializeField] protected GameObject secondaryParticlePrefab;
		
		[Cyberultimate.Unity.CommandProperty(get:true,set:false,"amount")]
		public static uint Amount { get; protected set; } = 0;
		public float MaxVisibleDistance { get; set; } = 3;
		
		public SimpleDirection Side { get; private set; }
		public bool Done { get; private set; }
		/// <summary>
		/// Might be null
		/// </summary>
		public DirectionMover DirectionMover { get; private set; }
		
		protected bool IsRegisteredForDeath { get; private set; }
		public bool SpawnParticleFlag { get; set; } = true;

		public event EventHandler OnKilled = delegate { };
		
		private float creationTime;

		
		public void SetSide(SimpleDirection side)
		{
			Side = side;
		}

		protected virtual void OnDestroy()
		{
			if (!IsRegisteredForDeath) Amount--;
		}

		protected virtual void Awake()
		{
			DirectionMover = this.GetComponent<DirectionMover>();
			Amount++;

			creationTime = Time.time;
		}
		private void Update()
		{
			//Give time for some objects (like clusters which have to spawn outside the screen) to float onto the screen
			if (Time.time - creationTime < 3)
				return;
			
			float trueVisibleDistance = MaxVisibleDistance;
			if (LevelManager.Current.LevelStatus != LevelManager.Status.Spawning)
			{
				trueVisibleDistance = 0;
				
			}
			Vector2 pos = this.transform.Get2DPos();
			Vector2 camBorder = CameraHelper.Current.CameraSize / 2;
			if (Math.Abs(pos.x)>camBorder.x + trueVisibleDistance || Math.Abs(pos.y)> camBorder.y + trueVisibleDistance )
				this.Kill();
		}

		public virtual bool Kill()
		{
			if (!this|| IsRegisteredForDeath) return false;
			Amount--;
			IsRegisteredForDeath = true;
			if(SpawnParticleFlag)
			{
				ParticleHelper.Spawn(secondaryParticlePrefab, this.transform.position, this.transform.parent);
			}

			Destroy(this.gameObject);
			OnKilled.Invoke(this,EventArgs.Empty);
			
			return true;
		}

		protected virtual void OnTriggerEnter2D(Collider2D other)
		{
			if (other.CompareTag("PlayerZone"))
				OnEnterZone(other);
		}
		protected virtual void OnExitZone(Collider2D other) { }
		protected virtual void OnEnterZone(Collider2D other){}
		
		protected virtual void OnTriggerExit2D(Collider2D other)
		{
			if (other.CompareTag("PlayerZone"))
			{
				OnExitZone(other);
			}
		}
	}


}

