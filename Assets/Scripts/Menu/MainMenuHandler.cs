using Cyberultimate.Unity;
using DG.Tweening;
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LetterBattle
{
	public class MainMenuHandler : MonoSingleton<MainMenuHandler>
	{
		[SerializeField] private Text version = null;

		[SerializeField]
		private Transform mainMainMenu = null;

		[SerializeField]
		private Image fullImg = null;
		public Image FullImg => fullImg;

		[Header("Ending Screen")]
		[SerializeField]
		private float endingScreenScale;

		[SerializeField]
		private Vector2 endingScreenPosition;


		[Header("Transition to Select Level")]
		[SerializeField]
		private float transitionDuration = 8f;

		[SerializeField]
		private Ease transitionEase = Ease.Linear;

		[SerializeField]
		private Vector2 finalPosition = new Vector2(-781, -340);
		public Vector2 FinalPosition => finalPosition;

		[Header("Menu Hideable")]
		[SerializeField]
		private GameObject mainMenu = null;

		[SerializeField]
		private GameObject selectAlbumMenu = null;

		[SerializeField]
		private GameObject levelsObj = null;

		[SerializeField]
		private Button[] buttons = null;

		[SerializeField]
		private Button albumFirstBtn = null;

		private Sequence mainSeq = null;

		[SerializeField]
		private GameObject playerLeg = null;
		public GameObject PlayerLeg => playerLeg;

		[SerializeField]
		private Transform title = null;

		[SerializeField]
		private GameObject cornerObj = null;

		[SerializeField]
		private Image[] breakLines = null;

		[Header("Mockup")]
		[SerializeField]
		private GameObject piece = null;

		[SerializeField]
		private MockupHandler mockup = null;

		[SerializeField]
		private float offsetY = 400;

		[Header(header: "Audio")]
		[SerializeField]
		private AudioSource menuSource = null;

		[SerializeField]
		private AudioSource soundSource = null;

		[Header(header: "Select Level Data")]
		[SerializeField]
		private Vector2 selectLvlScale = new Vector2(3, 3);
		public Vector2 SelectLvlScale => selectLvlScale;

		[SerializeField]
		private Vector3 selectLvlRotate = new Vector3(0, 0, 45);
		public Vector3 SelectLvlRotate => selectLvlRotate;

		private float logoStart;
		private float mainMenuStart;

		private Tween anim = null;
		public Tween Anim => anim;

		[SerializeField]
		private AlbumSelector selector = null;

		[Header(header: "Dialogues/Cutscenes")]
		[SerializeField]
		private GameObject dialogueUI = null;
		public GameObject DialogueUI => dialogueUI;

		[SerializeField]
		private CutsceneManager cutsceneManager = null;

		[SerializeField]
		private GameObject creditsObj = null;

		[SerializeField]
		private Button creditsBackBtn = null;
        
		private bool cutsceneMode;
        
		public static int SavedLevelIndex { get; set; } = -1;
		public static ICutsceneData CutsceneAsset { get; set; }


		protected override void Awake()
		{
			base.Awake();

			mainMainMenu.gameObject.SetActive(true);
			version.text = Application.version;
			selectAlbumMenu.SetActive(false);
			levelsObj.SetActive(false);
			mainMenu.SetActive(true);
			piece.SetActive(false);

			if (!GameManager.PlayerState.HasSeenStartCutscene)
			{
				GameManager.PlayerState.HasSeenStartCutscene = true;
				GameManager.TryToSave();
				LoadStartScreen();
				return;
			}

			menuSource.gameObject.SetActive(true);
		}

		public void StartGameWithAnimation(int lvlIndex)
		{
			LevelTransition(lvlIndex);
		}

		public void SetupCutscene(ICutsceneData cutscene)
		{
			cutsceneMode = true;
			menuSource.enabled = false;
			mainMenu.SetActive(false);
			title.gameObject.SetActive(false);

			dialogueUI.SetActive(true);
			cutsceneManager.Dispose();
			
			cutsceneManager.Load(cutscene);
			version.gameObject.SetActive(false);
			cutsceneManager.OnCutsceneEnded += OnCutsceneEnded;
		}

		public void LogoTransitionBasedOnChapter()
		{
			if (GameManager.WaitingForCutscene) return;
			void DoSmooth(int level)
			{
				fullImg.sprite = selector.Sprites[GameAsset.Current.GetChapter(level) ];
				selector.TextureTransition(GameAsset.Current.GetChapter(level), 0.5f);
				selector.ChangeChapter(GameAsset.Current.GetChapter(level));
			}
			if (cutsceneMode)
			{
				DoSmooth(Mathf.Max(0, GameManager.CurrentLevelNumber-1) );
			}
			else if (GameManager.LevelOfLastGame != null)
			{
				DoSmooth(GameManager.LevelNumberOfLastGame);
			}
			else
			{
				fullImg.sprite = selector.Sprites[GameAsset.Current.GetChapter(GameManager.PlayerState.LevelUnlocked)];
				selector.ChangeChapter(GameAsset.Current.GetChapter(GameManager.PlayerState.LevelUnlocked));
			}		
		}

		public Tween LevelTransition(int levelIndex = -1)
		{
			return TransitionHandler.Current.DoTransition().OnComplete(() =>
			{
				if (GameManager.IsGameOn)
					GameManager.CallCutsceneEnded();
				else
					GameManager.StartTheGame(levelIndex);
			});
		}

		private void OnDestroy()
		{
			mockup.OnExplode -= Mockup_OnExplode;

			if (cutsceneManager != null)
				cutsceneManager.OnCutsceneEnded -= OnCutsceneEnded;
		}
        
		private void OnCutsceneEnded(object sender, CutsceneAsset e)
		{
			GameManager.CallCutsceneEnded();
		}

		private void LoadStartScreen()
		{
			logoStart = title.transform.localPosition.y;
			mainMenuStart = mainMenu.transform.localPosition.y;

			foreach (Image img in breakLines)
			{
				img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
			}

			menuSource.gameObject.SetActive(false);
			piece.SetActive(true);
			cornerObj.SetActive(false);
			mainMenu.transform.localPosition = new Vector2(mainMenu.transform.localPosition.x, mainMenu.transform.localPosition.y - offsetY);
			title.transform.localPosition = new Vector2(title.transform.localPosition.x, title.transform.localPosition.y + offsetY);
			mainMenu.SetActive(false);
			playerLeg.SetActive(false);
			mockup.OnExplode += Mockup_OnExplode;
			mockup.Ready();
		}

        public void LoadEndingScreen()
        {
			Sequence seq = DOTween.Sequence();            
			// seq.Insert(0, Transition(new()));
		}

		private void Mockup_OnExplode(object sender, EventArgs args)
		{
			Sequence seq = DOTween.Sequence();

			menuSource.gameObject.SetActive(true);
			foreach (Image img in breakLines)
			{
				seq.Insert(0, img.DOFade(1, transitionDuration / 2.2f).SetEase(transitionEase));
			}

			cornerObj.SetActive(true);
			playerLeg.SetActive(true);
			mainMenu.SetActive(true);
			seq.Insert(0, mainMenu.transform.DOLocalMoveY(mainMenuStart, transitionDuration * 1.7f).SetEase(transitionEase));
			seq.Insert(0, title.transform.DOLocalMoveY(logoStart, transitionDuration * 1.7f).SetEase(transitionEase));
			seq.SetLink(this.gameObject);
		}

		private void Start()
		{		
			SteamHelper.Unlock("OTHER_ACHIEVEMENT_0");
			
			Text continueText = buttons[1].GetComponentInChildren<Text>();

			RectTransform pickRect = buttons[0].GetComponent<RectTransform>();
			RectTransform optionsRect = buttons[2].GetComponent<RectTransform>();

			if (GameManager.PlayerState.LevelUnlocked == 0 || GameManager.PlayerState.LevelUnlocked >= GameAsset.Current.Levels.Count)
			{
				buttons[1].gameObject.SetActive(false);

				pickRect.sizeDelta = new Vector2(pickRect.sizeDelta.x + 20, pickRect.sizeDelta.y);
				optionsRect.sizeDelta = new Vector2(pickRect.sizeDelta.x + 20, pickRect.sizeDelta.y);
			}
			else
			{
				buttons[1].gameObject.SetActive(true);
				continueText.text = string.Format(continueText.text, GameManager.PlayerState.LevelUnlocked);
			}

			EventSystemHandler.Current.Focus(buttons[0]);

			if (SavedLevelIndex != -1)
			{
				SetupCutscene(CutsceneAsset);

				SavedLevelIndex = -1;
			}
			if(!GameManager.WaitingForCutscene && SavedLevelIndex == -1)
				LogoTransitionBasedOnChapter();        
		}
        
		public void StartGame()
		{
			if (GameManager.WaitingForCutscene) return;
			LevelTransition();
		}
        
		public void ContinueGame()
		{
			if (GameManager.WaitingForCutscene) return;
			LevelTransition(GameManager.PlayerState.LevelUnlocked);
		}

		public void QuitGame() => Application.Quit(0);


		public void SelectLevelBtn()
		{
			anim = Transition(selectLvlRotate, selectLvlScale, finalPosition, true, transitionDuration, transitionEase, Ease.OutElastic);
		}

		public Tween Transition(Vector3 rotation, Vector2 scale, Vector2 move, bool enableAlbumSelection, float speed, Ease ease, Ease secondEase)
		{
			if (mainSeq != null)
			{
				mainSeq.Kill(true);
				mainSeq = null;
			}

			mainSeq = DOTween.Sequence();
			mainSeq.Insert(0, mainMainMenu.transform.DORotate(rotation, speed * 1.2f).SetEase(ease));
			mainSeq.Insert(0, mainMainMenu.transform.DOScale(scale, speed / 1.65f).SetEase(enableAlbumSelection ? secondEase : ease));
			mainSeq.Insert(0, mainMainMenu.transform.DOLocalMove(move, speed * 1.2f).SetEase(ease).OnStart(() =>
			{
				mainMenu.SetActive(!enableAlbumSelection);
				selectAlbumMenu.SetActive(enableAlbumSelection);
				levelsObj.SetActive(enableAlbumSelection);
			}));
			mainSeq.SetLink(this.gameObject);
			EventSystemHandler.Current.Focus(albumFirstBtn);
			return mainSeq;

		}
		public void BackBtn()
		{
			anim = Transition(new Vector3(0, 0, 0), new Vector2(1, 1), new Vector2(0, 0), false, transitionDuration / 1.2f, transitionEase, Ease.OutElastic);
			EventSystemHandler.Current.Focus(buttons[0]);
		}

		public void OpenWebsite(string url)
		{
			Application.OpenURL(url);
		}

		public void PlaySound(AudioClip clip)
		{
			soundSource.PlayOneShot(clip);
		}

		public void CreditsBtn()
		{
			creditsObj.SetActive(true);
			EventSystemHandler.Current.Focus(creditsBackBtn);
		}

		public void CreditsBackBtn()
		{
			creditsObj.SetActive(false);
			EventSystemHandler.Current.Focus(buttons[0]);
		}

		public void ForceBack(int chapter)
		{
			fullImg.sprite = selector.Sprites[chapter];
		}
	}
}