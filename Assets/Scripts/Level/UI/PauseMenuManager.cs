using DG.Tweening;
using LetterBattle.Utility;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LetterBattle
{
	public class PauseMenuManager : MonoBehaviour
	{
		[SerializeField]
		private Camera cam = null;
		[SerializeField]
		private SpriteRenderer starsSpriteRen = null;
		[SerializeField]
		private CanvasGroup gameplayUI = null;
		[SerializeField]
		private float duration = 6;
		[SerializeField]
		private float camSize = 2;
		[SerializeField]
		private Ease animEase;
		[SerializeField]
		private Ease secondEase;
		[SerializeField]
		private CanvasGroup pauseUI = null;
		[SerializeField]
		private CanvasGroup endUI = null;
		[SerializeField]
		private Vector2 scoreAndComboPos;
		[SerializeField]
		private Vector2 scoreAndComboNotMultiple;
		[SerializeField]
		private Vector2 scoreAndComboPosMoon;
		[SerializeField]
		private float delayUntilFunctional = 1f;
		[SerializeField]
		private Button pauseFirstBtn = null;
		[SerializeField]
		private Button endFirstBtn = null;

		[SerializeField]
		private Text rankText;
		
		public bool IsPaused { get; private set; }

		private float[] savedPositionsX;
		private CanvasGroup currentSelectedUI = null;
		private Vector2 startScoreAndComboPos;
		private Cooldown cooldown;
		private Sequence seq = null;
		private Sequence startSeq = null;
		private PlanetInfoContainer infoContainer = null;
		private int lastRandom = -1;

		protected void Awake()
		{
			pauseUI.gameObject.SetActive(false);
			endUI.gameObject.SetActive(false);
			pauseUI.alpha = 0f;
			endUI.alpha = 0f;
			LEvents.Base.OnLevelWon.Raw += OnLevelWon;
		}

#if !UNITY_EDITOR
		protected void OnApplicationFocus(bool focus)
		{
			if (!IsPaused)
			{
				SwitchPause();
			}
		}

		protected void OnApplicationPause(bool pause)
		{
			if (!IsPaused)
			{
				SwitchPause();
			}
		}
#endif

		private void OnLevelWon(object sender, EventArgs e)
		{
			EventSystemHandler.Current.Focus(endFirstBtn);
			currentSelectedUI = endUI;
			InitializePauseAnim(true);
		}


		protected IEnumerator Start()
		{
			// Patch for build version, not visible in editor (!):
			yield return null;

			LevelManager.Current.HPUI.alpha = 0;

			startSeq = DOTween.Sequence();
			seq.Insert(0, LevelManager.Current.HPUI.DOFade(1, duration / 2));
			seq.SetLink(this.gameObject).SetUpdate(true).SetEase(Ease.OutBack);


			if (LevelManager.Current.Bases.Count > 1)
			{
				savedPositionsX = new float[LevelManager.Current.Bases.Count];

				for (int i = 0; i < LevelManager.Current.Bases.Count; i++)
				{
					Planet planet = LevelManager.Current.Bases[i];
					if (planet.InfoUI.gameObject.activeSelf)
					{
						infoContainer = planet.InfoUI;
					}
					savedPositionsX[i] = planet.transform.localPosition.x;
					Vector2 vect = planet.transform.localPosition;
					vect.x = 0;
					planet.transform.localPosition = vect;
				}
				SetupReverseMultiplePlanets();
			}

			else
			{
				infoContainer = LevelManager.Current.Bases[0].InfoUI;
			}

			cooldown = new Cooldown(1.33f, unscaled: true);
			startScoreAndComboPos = infoContainer.ScoreDisplay.transform.localPosition;

		}

		protected void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape) && cooldown.Push())
			{
				SwitchPause();
			}
		}

		public void SwitchPause()
		{
			if (LevelManager.Current.LevelStatus != LevelManager.Status.Spawning && !IsPaused)
				return;

			IsPaused = !IsPaused;
			RefreshPause();
		}

		private void RefreshPause()
		{
			if (IsPaused)
			{
				TimeScaling.Status.Register(this, 0);
				EventSystemHandler.Current.Focus(pauseFirstBtn);
			}

			currentSelectedUI = pauseUI;
			InitializePauseAnim(IsPaused);

		}
		private void OnDestroy()
		{
			TimeScaling.Status.Unregister(this);
			LEvents.Base.OnLevelWon.Raw -= OnLevelWon;
		}

		private Tween InitializePauseAnim(bool isPaused)
		{

			if (!this.gameObject) return null;

			if (seq != null)
			{
				seq.Kill(true);
				seq = null;
			}

			currentSelectedUI.gameObject.SetActive(isPaused);

			seq = DOTween.Sequence();
			SetupPauseAnimation(isPaused);
			// multiple position planets:
			if (LevelManager.Current.Bases.Count > 1)
			{
				SetupMultiplePausePlanets(isPaused);
			}


			seq.Insert(0, DOVirtual.DelayedCall(delayUntilFunctional, () =>
			{
				if (isPaused == false)
				{
					TimeScaling.Status.Unregister(this);
				}
			}));
			return seq.SetLink(this.gameObject).SetUpdate(true);
		}

		private Tween SetupMultiplePausePlanets(bool isPaused)
		{
			startSeq.Kill(true);
			seq = DOTween.Sequence();
			LevelManager.Current.HPUI.alpha = isPaused ? 0 : 1;
			for (int i = 0; i < LevelManager.Current.Bases.Count; i++)
			{
				Planet planet = LevelManager.Current.Bases[i];
				seq.Insert(0, planet.transform.DOLocalMoveX(isPaused ? 0 : savedPositionsX[i], duration / 2.1f));
			}
			return seq.SetLink(this.gameObject).SetUpdate(true).SetEase(animEase);
		}

		private Tween SetupReverseMultiplePlanets()
		{
			seq = DOTween.Sequence();

			for (int i = 0; i < LevelManager.Current.Bases.Count; i++)
			{
				Planet planet = LevelManager.Current.Bases[i];
				seq.Insert(0, planet.transform.DOLocalMoveX(savedPositionsX[i], duration / 1.7f));
			}
			return seq.SetLink(this.gameObject).SetUpdate(true).SetEase(animEase);
		}

		private Tween SetupPauseAnimation(bool isPaused)
		{
			seq = DOTween.Sequence();

			SetupStrictPauseAnimation(isPaused);
			seq.Insert(0, gameplayUI.DOFade(isPaused ? 0 : 1, duration / 2f).SetEase(secondEase));
			infoContainer.Keyboard.gameObject.SetActive(!isPaused);

			return seq.SetLink(this.gameObject).SetUpdate(true);
		}

		private Tween SetupStrictPauseAnimation(bool isPaused)
		{
			seq = DOTween.Sequence();


			if (LevelManager.Current.Bases.Count > 1)
			{
				seq.Insert(0, infoContainer.ScoreDisplay.transform.DOLocalMove(isPaused ? scoreAndComboPos : startScoreAndComboPos, duration / 1.6f)).SetEase(animEase);

				if (lastRandom == -1)
				{
					lastRandom = UnityEngine.Random.Range(0, LevelManager.Current.Bases.Count);
				}

				LevelManager.Current.Bases[lastRandom]
				.SpriteRen.transform.GetChild(0).gameObject.SetActive(isPaused);

				if (!isPaused)
				{
					lastRandom = -1;
				}
			}
			else
			{
				if (LevelManager.Current.Bases[0].PlanetType == Planet.Type.Moon)
				{
					seq.Insert(0, infoContainer.ScoreDisplay.transform.DOLocalMove(isPaused ? scoreAndComboPosMoon : startScoreAndComboPos, duration / 1.6f)).SetEase(animEase);
				}
				else
				{
					seq.Insert(0, infoContainer.ScoreDisplay.transform.DOLocalMove(isPaused ? scoreAndComboNotMultiple : startScoreAndComboPos, duration / 1.6f)).SetEase(animEase);
				}
				
			}

			seq.Insert(0, cam.DOOrthoSize(isPaused ? camSize : CameraHelper.CamOrthographicSize, duration).SetEase(animEase));
			seq.Insert(0, starsSpriteRen.DOColor(isPaused ? GameAsset.Current.Pallete.GetColor(ColorType.Secondary) : Color.white, duration / 1.6f).SetEase(animEase));
			seq.Insert(0, currentSelectedUI.DOFade(isPaused ? 1 : 0, duration / 2f).SetEase(secondEase));
			return seq.SetLink(this.gameObject).SetUpdate(true);
		}

		public void Resume() => SwitchPause();
		public void Restart()
		{
			duration /= 2.5f;
			SetupStrictPauseAnimation(false).OnComplete(
			  () => GameManager.RestartLevel());
		}
		public void Settings()
		{

		}

		private Tween GetTweenTransition()
		{
			seq = DOTween.Sequence();
			if (LevelManager.Current.Bases.Count > 1)
			{
				LevelManager.Current.Bases[lastRandom]
.SpriteRen.transform.GetChild(0).gameObject.SetActive(false);
			}
			else
			{
				seq.Insert(0, LevelManager.Current.HPUI.DOFade(0, duration / 2.66f).SetEase(secondEase));
			}
			seq.Insert(0, currentSelectedUI.DOFade(0, duration / 2.66f).SetEase(secondEase));
			seq.Insert(0, GameOverManager.Current.GetLevelTransition());
			return seq.SetLink(this.gameObject).SetUpdate(true);
		}


		public void ViewLevels()
		{
			
			GetTweenTransition().OnComplete(()
				=> GameManager.StopTheGame());
		}

		public void Retry()
		{
			GetTweenTransition().OnComplete(() => GameManager.RestartLevel());
		}

		public void Next()
		{
			GetTweenTransition().OnComplete(()
				=> FinalizeLevel());
		}

		private void FinalizeLevel()
		{

			GameManager.FinalizeLevel(-1, true);

		}
	}


}


