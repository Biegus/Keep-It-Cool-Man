using Cyberultimate;
using Cyberultimate.Unity;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace LetterBattle
{
	public class ScoreDisplayer : MonoBehaviour
	{
		[SerializeField] private GameObject ComboTextPrefab;

		[SerializeField]
		private float resetTime;

		[SerializeField]
		private Text textEntity;
		[SerializeField]
		private Text comboEntity;
		[SerializeField]
		private Ease scoreAnim = Ease.InOutCubic;
		private string formatedScore;
		private string formatedCombo;
		private Tween scoreTween = null;
		private CyberCoroutine scoreCoroutine = null;
		private Vector3 lastHitLetterPos;

		public void SetupTextAlignment(bool isRight)
		{
			textEntity.alignment = isRight ? TextAnchor.LowerRight : TextAnchor.LowerLeft;
			comboEntity.alignment = isRight ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
		}


		private void Awake()
		{
			LEvents.Base.OnLetterDestroyedByPlayerInteraction.Raw += OnLetterDestroyed;
			LEvents.Base.OnComboChanged.Raw += OnRefreshCombo;
			formatedScore = textEntity.text;
			formatedCombo = comboEntity.text;

		}
		private void Start()
		{
			ScoreSystem.Current.ResetTime = resetTime;
			ScoreSystem.Current.Score.OnValueChanged += OnRefreshScore;
			RefreshCombo();
			RefreshScoreText();
		}
		private void OnDestroy()
		{
			if (ScoreSystem.Current)
				ScoreSystem.Current.Score.OnValueChanged -= OnRefreshScore;
			LEvents.Base.OnComboChanged.Raw -= OnRefreshCombo;
			LEvents.Base.OnLetterDestroyedByPlayerInteraction.Raw -= OnLetterDestroyed;
		}
		private void OnRefreshScore(object obj, LockValue<int>.AnyValueChangedArgs args)
		{
			if (args.Action == LockValue<int>.Action.Add)
			{
				scoreTween?.Kill();
				Sequence seq = DOTween.Sequence();
				seq.Append(textEntity.transform.DOScale(Vector3.one * 1.5f, 0.2f).SetEase(scoreAnim));
				seq.Append(textEntity.transform.DOScale(Vector3.one, 0.2f).SetEase(scoreAnim));
				seq.SetUpdate(true);
				seq.SetLink(this.gameObject);
				scoreTween = seq;
			}
			else
			{
				scoreCoroutine?.Stop();
				IEnumerator<IWaitable> CFlicker()
				{

					for (int i = 0; i < 5; i++)
					{
						if (textEntity == null)
						{
							yield break;
						}

						this.textEntity.color = new Clr(this.textEntity.color, this.textEntity.color.a == 1 ? 0 : 1);

						yield return Yield.Wait(0.05f);
					}

				}
				scoreCoroutine = CorController.Base.Start(CFlicker(), this.textEntity).OnEnd((c) =>
				{
					if (this.textEntity != null)
						this.textEntity.color = new Clr(this.textEntity.color, 1);
				});
			}
			RefreshScoreText();


		}
		private void OnRefreshCombo(object obj, LEvents.ComboChangeArgs combo)
		{
			RefreshCombo();

			if (combo.CurrentCombo > combo.PreviousCombo &&
				combo.CurrentCombo == Mathf.Floor(combo.CurrentCombo))
			{
				var comboTextEntity = Instantiate(ComboTextPrefab)
					.GetComponent<ComboText>();
				comboTextEntity.gameObject.SetActive(false);
				CorController.Base.DelayOneFrame(() =>// lol this is convoluted but sometimes, the pos was not yet updated
				{
						comboTextEntity.gameObject.SetActive(true);
						comboTextEntity.Init((int)combo.CurrentCombo, lastHitLetterPos);
					}, this)
			 ;

			}
		}
		private void RefreshScoreText()
		{
			textEntity.text = string.Format(formatedScore, ScoreSystem.Current.Score.Value);
		}

		private void RefreshCombo()
		{
			comboEntity.text = string.Format(formatedCombo, ScoreSystem.Current.Combo);
		}

		private void OnLetterDestroyed(object sender, LetterActionArgsOnDeath args)
		{
			lastHitLetterPos = args.ActionLetter.transform.position;
		}
	}
}