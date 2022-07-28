using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyberultimate;
using Cyberultimate.Unity;
using System;
using DG.Tweening;
using LetterBattle.Utility;

namespace LetterBattle
{
	public class AudioBeatClicker : MonoBehaviour
	{
		
		private AudioSource audioSource = null;
		private Zone zone;

		[SerializeField]
		private Vector2 pitchRangeMaxMin = new Vector2(0.8f, 1.25f);
	
		protected void Awake()
		{
			audioSource = this.GetComponent<AudioSource>();

			LEvents.Base.OnLetterDestroyedByPlayerInteraction.Raw += OnDestroyedLetter;
		}

		protected void Start()
		{
			zone = GameObject.FindGameObjectWithTag("PlayerZone").GetComponent<Zone>();
		}

		protected void OnDestroy()
		{
			LEvents.Base.OnLetterDestroyedByPlayerInteraction.Raw -= OnDestroyedLetter;
		}

		private void OnDestroyedLetter(object sender, LetterActionArgsOnDeath e)
		{
			if (e.DeathType == ActionLetter.DeathType.KilledByPlayer)
			{
				SimpleDirection simpleDir = e.ActionLetter.Side;

				audioSource.pitch = UnityEngine.Random.Range(pitchRangeMaxMin.x, pitchRangeMaxMin.y);
				audioSource.PlayOneShot(GameAsset.Current.CorrectSounds.Map[simpleDir]);

			}
		}
	}
}

