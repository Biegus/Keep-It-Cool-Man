using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyberultimate;
using Cyberultimate.Unity;
using UnityEngine.UI;

namespace LetterBattle
{
	public class PopupManager : MonoSingleton<PopupManager>
	{
		[SerializeField]
		private GameObject levelInfoObj = null;

		[SerializeField]
		private Text scoreTxt = null;

		[SerializeField]
		private Text comboTxt = null;

		[SerializeField]
		private Text lvlTxt = null;

		[SerializeField]
		private Text levelName = null;

		[SerializeField]
		private Text rankText = null;

		private bool isPop;

		protected void Start()
		{
			Refresh(false, 0);
		}

		public void SwitchPopup(bool isTrue, int levelNum)
		{
			if (isTrue == isPop)
			{
				return;
			}

			isPop = isTrue;
			Refresh(isTrue, levelNum);
		}

		private void Refresh(bool isTrue, int levelNum)
		{
			int scoreNumber = 0;
			float combo = 0;
			RankLevel rank = RankLevel.Unknown;
			if (GameManager.PlayerState.IsLevelUnlocked(levelNum))
			{
				if (GameManager.PlayerState.Scores.Count > levelNum)
				{
					scoreNumber = GameManager.PlayerState.Scores[levelNum].Number;
					combo = GameManager.PlayerState.Scores[levelNum].Combo;
					rank = GameManager.PlayerState.Scores[levelNum].Rank;
				}

				comboTxt.text = $"Combo: x {combo}";
				scoreTxt.text = $"Score: {scoreNumber}";
				RankTextData rankData = rank.GetTextData();
				rankText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(rankData.Color)}>{rankData.Name}</color>";
			}

			else
			{
				scoreTxt.text = "LOCKED";
				comboTxt.text = "";
				rankText.text = "";
			}

			levelInfoObj.SetActive(isTrue);
			levelName.text = GameAsset.Current.Levels[levelNum].ToString();
			lvlTxt.text = levelNum.ToString();
		}
	}
}

