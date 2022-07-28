using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LetterBattle
{
	public class CanvasManager : MonoBehaviour
	{
		public enum VisibilityGroup
		{
			Everything = 0,
			UserUI = 1,
			NoDetailsUI = 2,
			Nothing = 3
		}

		[SerializeField]
		private CanvasGroup defaultInfoCanvas = null;

		[SerializeField]
		private DebugGUI debugCanvas = null;

		public VisibilityGroup IsVisible { get; private set; }

		protected void Update()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				SwitchCanvases((VisibilityGroup)(((int)IsVisible++ + 1) % 4));
			}
		}

		public void SwitchCanvases(VisibilityGroup group)
		{
			switch (group)
			{
				case VisibilityGroup.Everything:
					LevelManager.Current.Bases[0].HpUICanvas.alpha = 1;
					LevelManager.Current.Bases[0].InfoUICanvas.alpha = 1;
					debugCanvas.gameObject.SetActive(true);
					defaultInfoCanvas.alpha = 1;
					break;

				case VisibilityGroup.UserUI:
					debugCanvas.gameObject.SetActive(false);
					break;

				case VisibilityGroup.NoDetailsUI:
					defaultInfoCanvas.alpha = 0;
					break;

				default:
					LevelManager.Current.Bases[0].HpUICanvas.alpha = 0;
					LevelManager.Current.Bases[0].InfoUICanvas.alpha = 0;
					break;
			}


		}
	}
}

