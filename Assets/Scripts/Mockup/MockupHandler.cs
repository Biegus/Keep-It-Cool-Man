using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace LetterBattle
{
	public class MockupHandler : MonoSingleton<MockupHandler>
	{
		[SerializeField]
		private Transform logo = null;
		
		[SerializeField]
		private MockupLetter mockupLetter = null;
		[SerializeField]
		private Text shootInfo = null;
		private Tween shootInfoTween = null;
		[SerializeField]
		private float duration = 2.2f;
		[SerializeField]
		private Canvas UI = null;
		[SerializeField]
		private UnityEngine.Rendering.Universal.Light2D lighto = null;
		[SerializeField]
		private GameObject particlePrefab = null;
		[SerializeField]
		private Rigidbody2D pieceBody = null;
		[SerializeField]
		private float angleError = 20f;
		[SerializeField]
		private float explodePieceForce = 115f;
		[SerializeField]
		private float lightStrength = 0.785f;
		[SerializeField]
		private int howManySpawn = 4;
		[SerializeField]
		private float explodeLimit = 2;
		[SerializeField]
		private string shootInfoText = "There are more of them! Quickly! Type <color=red>s</color> as fast as you can! We have to shoot these god damn letters now!";
		
		public List<MockupLetter> LettersInZone { get; private set; } = new List<MockupLetter>();
		private List<MockupLetter> SpawnedLetters = new List<MockupLetter>();
		
		private Image pieceImg = null;
		private bool spawn = true;
		public event EventHandler OnExplode = delegate { };

		protected override void Awake()
		{
			base.Awake();
			pieceImg = pieceBody.GetComponent<Image>();
		}

		public void Ready()
		{
			shootInfo.gameObject.SetActive(true);
			SpawnLetter();
		}

		private void SpawnLetter()
		{
			MockupLetter let = Instantiate(mockupLetter, (Vector2)logo.localPosition + new Vector2(10, 0), Quaternion.identity, UI.transform);
			let.Mockup = this;
			let.DirMov.Direction = Randomer.Base.NextRandomRotation(let.transform.Get2DPos(), angleError, logo.localPosition);

			let.DirMov.SpeedPlain = 9f;
			DOVirtual.DelayedCall(0.25f, () =>
			{
				let.DirMov.SpeedPlain = 0.33f;
			},true);
			
			SpawnedLetters.Add(let);
		}

	

		public void AddLetter(MockupLetter letter)
		{
			if (spawn == false)
			{
				return;
			}

			LettersInZone.Add(letter);
			TimeScaling.Status.Register(this, 0.1f);

			if (shootInfoTween != null)
			{
				return;
			}	

			shootInfoTween = shootInfo.transform.DOLocalMoveY(-360, duration).SetEase(Ease.OutCubic)
				.SetLink(this.gameObject).SetUpdate(true);
		}

		public void RemoveLetter(MockupLetter letter)
		{
			LettersInZone.Remove(letter);
			SpawnedLetters.Remove(letter);
			ParticleHelper.Spawn(particlePrefab, letter.transform.position);
			Destroy(letter.gameObject);


			if (lighto.intensity > explodeLimit)
			{
				Explode();
			}

			if (LettersInZone.Count == 0)
			{
				TimeScaling.Status.Unregister(this);
			}

			if (SpawnedLetters.Count == 0 && spawn)
			{
				howManySpawn *= 2;
				for (int i = 0; i < howManySpawn; i++)
				{
					SpawnLetter();
				}

				shootInfo.text = shootInfoText;
			}
		}

		private void SpawnLaser(MockupLetter letter)
		{
			LaserManager.Spawn(logo.Get2DPos(),letter.transform.Get2DPos());
		}


		private void Explode()
		{
			if (spawn == false)
			{
				return;
			}

			spawn = false;
			StopAllCoroutines();
			shootInfoTween.Kill(true);
			shootInfoTween = shootInfo.transform.DOLocalMoveY(-800, duration).SetEase(Ease.OutCubic).SetLink(this.gameObject);
			pieceBody.AddForce(Vector2.right * explodePieceForce, ForceMode2D.Impulse);
			Tween k = pieceImg.DOFade(0f, 7f).SetLink(pieceImg.gameObject).SetEase(Ease.OutCubic).OnComplete(() => {
				Destroy(pieceImg.gameObject);
			});


			LettersInZone.Clear();
			OnExplode(this,EventArgs.Empty);
		}

		protected void OnDestroy()
		{
			TimeScaling.Status.Unregister(this);
		}

		protected void Update()
		{
			if (Input.GetKeyDown(KeyCode.S))
			{
				OnKeyPress();
			}

		}

		public void OnKeyPress()
		{
			if (LettersInZone.Count == 0)
			{
				return;
			}

			MockupLetter letter = Randomer.Base.NextRandomElement(LettersInZone);
			RemoveLetter(letter);
			SpawnLaser(letter);
			lighto.intensity += lightStrength;

		}
	}


}

