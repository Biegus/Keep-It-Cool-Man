using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cyberultimate;
using Cyberultimate.Unity;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

namespace LetterBattle
{
	public class GameOverManager : MonoSingleton<GameOverManager>
	{
		[SerializeField]
		private Camera cam = null;

		[SerializeField]
		private SpriteRenderer starsSprite = null;

		[SerializeField]
		private float endPositionY = 15f;

		[SerializeField]
		private float duration = 7f;

		[SerializeField]
		private Ease ease = Ease.OutElastic;

		private Tween t;

		[SerializeField]
		private Canvas thisCanvas = null;

		[SerializeField]
		private Vector2 minScale;
		[SerializeField]
		private Vector2 maxScale;

		[SerializeField]
		private Vector2 move;

		[SerializeField]
		private float resetDuration = 2f;

		[SerializeField]
		private Button firstBtn = null;

		[SerializeField]
		private Transform planetList = null;

		[SerializeField]
		private Image deadPlanetPrefab = null;

		private int length = 1;

		[SerializeField]
		private float scalePlanetMultiply = 0.01f;

		[SerializeField]
		private CanvasGroup gameOverUI = null;

		[SerializeField]
		private Text scoreTxt = null;

		[SerializeField]
		private Text comboTxt = null;

		[SerializeField]
		private CanvasGroup infoUI = null;

		private Sprite[] sprites;
		private bool called;
		private bool restartedAlready = false;

		[SerializeField]
		private Vector2 camMoveMoon;
		protected override void Awake()
		{
			base.Awake();
			gameOverUI.gameObject.SetActive(false);
			gameOverUI.alpha = 0f;
			LEvents.Base.OnLevelLost.Raw += OnLevelEnded;
			LEvents.Base.OnLevelStarted.Raw += OnLevelStarted;
		}

		private void OnLevelStarted(object sender, EventArgs e)
		{
			sprites = LevelManager.Current.Bases.Select(item => item.SpriteRen.sprite).ToArray();
		}

		private void OnDestroy()
		{
			LEvents.Base.OnLevelLost.Raw -= OnLevelEnded;
			LEvents.Base.OnLevelStarted.Raw -= OnLevelStarted;
		}

		private void OnLevelEnded(object sender,EventArgs args)
		{
			if(called)
				Debug.LogError("level ended event was called multiple times");
			called = true;
			InternalOver();
		}

		private void SetupDeadPlanetsList()
		{
			planetList.KillAllChildren();

			foreach (Sprite planet in sprites)
			{
				Image img = Instantiate(deadPlanetPrefab, planetList);
				img.sprite = planet;
			}
		}

		private void InternalOver()
		{
			scoreTxt.text = ScoreSystem.Current.GenerateScoreData().Number.ToString();
			comboTxt.text = string.Format(comboTxt.text, ScoreSystem.Current.GenerateScoreData().Combo.ToString());

			gameOverUI.gameObject.SetActive(true);
			gameOverUI.DOFade(1, duration * 1.5f).SetLink(this.gameObject);

			SetupDeadPlanetsList();

			starsSprite.DOColor(Color.black, duration / 2f).SetLink(this.gameObject);
			SpaceSpinner spin = starsSprite.GetComponent<SpaceSpinner>();
			t = DOVirtual.Float(spin.Duration, 200, duration * 13f, (e) =>
			{
				spin.Duration = e;
				if (e >= 15.01)
				{
					Destroy(spin.gameObject);
					spin = null;
					t.Kill(false);
				}
			}).SetEase(ease).SetLink(spin.gameObject);
			cam.transform.DOLocalMoveY(endPositionY, duration).SetEase(ease).SetLink(this.gameObject);
			starsSprite.transform.DOLocalMoveY(endPositionY, duration).SetEase(ease).SetLink(this.gameObject);
			EventSystemHandler.Current.Focus(firstBtn);
		}

		private void AnimButtionAndRestart()
		{
			GetLevelTransition(true);
			thisCanvas.transform.DOScale(new Vector2(maxScale.x - length * scalePlanetMultiply - scalePlanetMultiply, maxScale.y - length * scalePlanetMultiply - 0.01f), resetDuration / 1.2f)
				.OnComplete(() => GameManager.RestartLevel()).SetEase(ease).SetLink(this.gameObject);
			thisCanvas.transform.DOMove(new Vector2(move.x, move.y + (length - 1) * 0.7f + (length - 1)), resetDuration / 1.3F).SetEase(ease).SetLink(this.gameObject);
		}

		public void RestartLvl()
		{
			if (restartedAlready) return;
			restartedAlready = true;
			thisCanvas.transform.DOScale(minScale, resetDuration / 1.8F).OnComplete(() => AnimButtionAndRestart()).SetEase(ease).SetLink(this.gameObject);
		}

		public Tween GetLevelTransition(bool silent = false)
		{
			Sequence seq = DOTween.Sequence();
			seq.Insert(0, infoUI.DOFade(0, duration / 8));
			if (LevelManager.Current.Bases[0].PlanetType == Planet.Type.Moon && LevelManager.Current.Bases.Count < 2)
			{
				seq.Insert(0, TransitionHandler.Current.DoTransition(silent: silent, camMoveMoon));
			}

			else
			{
				seq.Insert(0, TransitionHandler.Current.DoTransition(silent: silent));
			}



			return seq;

		}

	}
}

