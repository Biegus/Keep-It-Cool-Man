using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//TODO REFACTOR
namespace LetterBattle
{
	public class SelectionButton : MonoBehaviour
	{
		[SerializeField]
		private Image frame = null;
		[SerializeField]
		private Text levelText = null;
		[SerializeField]
		private Button btn = null;
		[SerializeField]
		private float duration = 5f;
		[SerializeField]
		private AlbumSelector selector;
		[SerializeField]
		private Image dot = null;
		
		private Navigation defNavi;
		private int num;

		protected void Awake()
		{
			defNavi = btn.navigation;
			frame.transform.DoRotateAboutZ(360, duration).SetLoops(-1).SetEase(Ease.Linear).SetLink(this.gameObject);

			num = int.Parse(levelText.text);
			btn.onClick.AddListener(Call);

			selector.OnChapterChanged += RefreshState;

		}
		private void Start()
		{
			RefreshState(null, selector.ChapterSelected);
		}

		private void OnDestroy()
		{
			selector.OnChapterChanged -= RefreshState;
		}

		private void RefreshState(object obj, int chapter)
		{
			if ( GameAsset.Current.Chapters[chapter].Levels.Count - 1 == num)
			{
				Navigation nav = btn.navigation;
				nav.selectOnRight = selector.Buttons[0];
				btn.navigation = nav;
			}
			else
			{
				btn.navigation = defNavi;
			}

			bool isLevelUnlocked = GameManager.PlayerState.IsLevelUnlocked(chapter, num);
			dot.color = isLevelUnlocked ? GameAsset.Current.Pallete.GetColor(ColorType.Success) : GameAsset.Current.Pallete.GetColor(ColorType.Danger);
			bool bul = (GameAsset.Current.Chapters[chapter].Levels.Count - 1 >= num);
			btn.gameObject.SetActive(bul);
			frame.gameObject.SetActive(bul);
			this.levelText.text = $"{GameAsset.Current.FromCertainToRaw(chapter, num)}";
		}



		public void Call()
		{
			if (GameManager.PlayerState.IsLevelUnlocked(selector.ChapterSelected, num))
			{
				MainMenuHandler.Current.Transition(new Vector3(0, 0, 0), new Vector2(1, 1), new Vector2(0, 0), false, 0.8f, Ease.OutQuint, Ease.OutElastic).OnComplete(() => ControlTransition());
			}

			else
			{
				CameraHelper.Current.ShakeScreen();
			}
		}
		
		private void ControlTransition()
		{
			MainMenuHandler.Current.StartGameWithAnimation((GameAsset.Current.FromCertainToRaw(selector.ChapterSelected, num)));
		}
	}
}

