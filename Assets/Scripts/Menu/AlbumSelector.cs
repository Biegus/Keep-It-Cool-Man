using DG.Tweening;
using LetterBattle.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LetterBattle
{
	public class AlbumSelector : MonoBehaviour
	{
        [Serializable]
		private class Operation
        {
			[SerializeField]
			private string operationName;
			[SerializeField]
			private string startYear;
			[SerializeField]
			private string endYear;

			public string OperationName => operationName;

			public string GetYearRange()
            {
				return $"{startYear}-{endYear}";
			}
        }

		[SerializeField] private Button[] buttons = new Button[0];
		[SerializeField] private Sprite[] sprites = new Sprite[0];
		[SerializeField] private Operation[] operations;

		[SerializeField]
		private Image recordImage;
		[SerializeField]
		private Sprite lastRecordSprite;
		[SerializeField]
		private Sprite normalRecordSprite;

		[SerializeField]
		private Text operationTitle;
		[SerializeField]
		private Text operationDateRange;

		[SerializeField] private Button btnFirstLvl = null;
		public IReadOnlyList<Sprite> Sprites => sprites;
		[SerializeField]
		private Transform mainMainMenu = null;

		[SerializeField]
		private float time;

		[SerializeField]
		private Ease rotateEase = Ease.InOutCirc;

		[SerializeField]
		private Ease fadeImgEase = Ease.InOutBack;

		public int ChapterSelected { get; private set; }
		public event EventHandler<int> OnChapterChanged = delegate { };

		public IReadOnlyList<Button> Buttons => buttons;

		private Sequence transitionInProgress;
		private int val;

		private void Awake()
		{
			val = buttons.Length - (buttons.Length - (GameAsset.Current.GetChapter(GameManager.PlayerState.LevelUnlocked) + 1));
			for (var index = 0; index < val; index++)
			{
				var button = buttons[index];
				button.transform.GetChild(0).gameObject.SetActive(false);
				button.image.color = new Color(1, 1, 1, 1);
				button.gameObject.SetActive(true);
				var rect = button.GetComponent<RectTransform>();
				int cur = index;
				var index1 = index;
				button.onClick.AddListener(() =>
				{
					MainMenuHandler.Current.Anim.Kill(true);
					TriggeredTweenEffect tweenEff = button.GetComponent<TriggeredTweenEffect>();
					tweenEff.enabled = false;
					button.enabled = false;

					transitionInProgress?.Kill(true);

					transitionInProgress = DOTween.Sequence();
					transitionInProgress.Insert(0, mainMainMenu.DoRotateAboutZ(360, time * 1.55f, () =>
					{
						// mechanical stuff
						ChangeChapter(cur);
						EventSystemHandler.Current.Focus(btnFirstLvl);
						TurnBack(button, tweenEff);
					}, 70).SetEase(rotateEase));

					transitionInProgress.Insert(0, TextureTransition(index1, time * 2.4f));

					transitionInProgress.SetLink(this.gameObject);


				});
			}
		}

		public Tween TextureTransition(int chapter, float textureTime)
		{
            Sequence seq = DOTween.Sequence();
			MainMenuHandler.Current.FullImg.material.SetTexture("_OtherTex", sprites[chapter].texture);
			return DOVirtual.Float(0, 1, textureTime, (v) =>
			{
				MainMenuHandler.Current.FullImg.material.SetFloat("_V", v);
			}).SetLink(this.gameObject).OnComplete(() =>
			{
				MainMenuHandler.Current.FullImg.sprite = sprites[chapter];// this should not get this from menu class
				MainMenuHandler.Current.FullImg.material.SetFloat("_V", 0);
			}).SetLink(gameObject).SetEase(fadeImgEase);
		}
     
		private void Start()
		{
			int lv;
			if (GameManager.LevelOfLastGame)
			{
				lv = GameManager.LevelNumberOfLastGame;
			}
			else
			{
				lv = GameManager.PlayerState.LevelUnlocked;
			}
			ChangeChapter( GameAsset.Current.GetChapter(lv));
		}
		public void ChangeChapter(int ind)
		{
			if (ind == 4)
			{
				recordImage.sprite = lastRecordSprite;
			}
			else
			{
				recordImage.sprite = normalRecordSprite;
			}
			operationTitle.text = operations[ind].OperationName;
			operationDateRange.text = operations[ind].GetYearRange();
			ChapterSelected = ind;
			// the one problem that is now that look of planet is not tied to the change chapter 
			// it means that selected chapter and graphic could in theory not match,
			// it might take some time to fix  it is not super important, tho if this starts to create more problem this is the thing that should be done
			OnChapterChanged(this, ind);
		}

		private void TurnBack(Button btn, TriggeredTweenEffect tween)
		{
			tween.enabled = true;
			btn.enabled = true;
		}
	}
}

