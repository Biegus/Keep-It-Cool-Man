using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace LetterBattle
{

	public class CutsceneManager : MonoBehaviour
	{
		public enum StartMode
		{
			Never,
			Awake,
			OnEnable
		}
		[SerializeField] [ValidateNotNull] private CharactersDefinitionAsset definition;
		[SerializeField] [Range(0, 0.3f)] private float timePerCharacter = 0.1f;
		[SerializeField] private CutsceneAsset defaultAsset;
		[SerializeField] private StartMode startMode = StartMode.Awake;
		[SerializeField] [NaughtyAttributes.Required] private TMP_Text textEntity;
		[SerializeField] [NaughtyAttributes.Required] private TMP_Text whoTextEntity;
		[SerializeField] [NaughtyAttributes.Required()] private Image profileEntity;
		[SerializeField] [NaughtyAttributes.Required()] private GameObject mainBox = null;
		[SerializeField] private AudioSource musicSource;
		[SerializeField] private AudioSource talkingSource;

		[SerializeField] [Required] private GameObject appearText;
		[SerializeField] private Image visualImagePrefab;
		[SerializeField] private List<Transform> visualPoints;
		public ICutsceneData Asset { get; private set; }
		public int CutSceneIndex { get; private set; }
		public float Progress { get; private set; }
		public bool InProgress { get; private set; }
		public event EventHandler<CutsceneAsset> OnCutsceneEnded = delegate { };

		private Tween specialTween;
		private float localSpeedModif = 1;
		private float pauseTime = 0;
		private string whipedText;
		private float startCutsceneTime;
		private Vector2 baseProfilePosition;
		private List<GameObject> visualElements = new();
		
		protected virtual void Awake()
		{
			appearText.SetActive(false);
			if (startMode == StartMode.Awake) TryToLoadDefaultAsset();
			baseProfilePosition = profileEntity.transform.position;
		}
		private void TryToLoadDefaultAsset()
		{
			if (defaultAsset != null)
				Load(defaultAsset);
		}
		protected virtual void OnEnable()
		{
			if (startMode == StartMode.OnEnable) TryToLoadDefaultAsset();
		}

		public void HandleTag(string tag)
		{
			Regex splitRegex = new Regex(@"^(?'name'\w+?)(_(?'arg'.*))?$");
			Match match = splitRegex.Match(tag);
			string name = match.Groups["name"].Value;
			string arg = match.Groups["arg"].Value;

			float ParseArgs()
			{
				float parsed = float.Parse(arg,CultureInfo.InvariantCulture);
					
				if (parsed <= 0)
				{
					Debug.LogError("argument should be positive");
					return 1;
				}

				return parsed;
			}
			switch (name)
			{
				case "speed":
				{
					if (arg == "r")
					{
						localSpeedModif = 1;
						return;
					}

					float parsed = ParseArgs();
					localSpeedModif = parsed;

					return;
				}
				case "quake":
					int times = 40;
					if (!string.IsNullOrEmpty(arg))
					{
						times = int.Parse(arg);
					}
					this.PlayStatic(CameraHelper.Current.ShakeScreen(times));

					return;
				case "pause":
				{
					float parsed = ParseArgs();
					pauseTime = parsed;
					return;
				}
				case "changeback":
				{
					int parsed = int.Parse(arg);
					MainMenuHandler.Current.ForceBack(parsed);
					return;
				}
				case "shakeavatar":
					
					this.PlayStatic(Effects.ShakeObj(this.profileEntity.gameObject, baseProfilePosition, int.MaxValue,0.01f,delay:0f,limit:this));
					break;
					
					
			}
		}
		public void Load(ICutsceneData asset)
		{

			if (asset == null)
				throw new ArgumentNullException(nameof(asset));
			if (asset?.Elements.Count == 0)
				throw new ArgumentException("Asset cannot be empty");
			if (InProgress)
				Dispose();
			if (asset.Back != -1)
				MainMenuHandler.Current.ForceBack(asset.Back);
			if(musicSource && asset.Clip)	
				musicSource.PlayOneShot(asset.Clip);
			this.Asset = asset;
			CutSceneIndex = -1;
			Progress = 0;
			startCutsceneTime = Time.time;
			InProgress = true;
		}
		private void OnGUI()
		{
			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			GUIStyle textStyle = new GUIStyle("label")
			{
				normal =
				{
					textColor = Color.black
				},
				fontSize = 20
				
			};
			GUIStyle buttonStyle = new GUIStyle("button")
			{
				fontSize = 22
			};
			GUILayout.BeginVertical("box");
				GUILayout.Label("CUTSCENE DEBUG",textStyle);
				GUILayout.Label($"index: {this.CutSceneIndex}",textStyle);
				GUILayout.Label($"progres: {this.Progress}",textStyle);
				GUILayout.Label($"is in progress: {this.InProgress}",textStyle);
				GUILayout.Label($"time per character {this.timePerCharacter}",textStyle);
				if (GUILayout.Button("Next",buttonStyle))
				{
					MoveUp();
				}
				#if UNITY_EDITOR
				if (GUILayout.Button("Open asset", buttonStyle))
				{
					UnityEditor.EditorApplication.ExecuteMenuItem("Window/General/Inspector");
					UnityEditor.Selection.activeObject = (UnityEngine.Object) Asset;
				}
				#endif
			GUILayout.EndHorizontal();
			#endif
		}
		public  void PlayStatic( CyberCoroutine cor)
		{
			var index = this.CutSceneIndex;
			
			this.gameObject.DoUpdate(() =>
			{
				if (index != this.CutSceneIndex)
					cor.Stop();
			});
		}
		private void Update()
		{
			if (!InProgress) return;
		
			if (CutSceneIndex == -1)
			{
				CutSceneIndex++;
				Prepare();
			}
			if (pauseTime > 0)
			{
				pauseTime -= Time.deltaTime;
				return;
			}

			
			if (Asset.Elements[CutSceneIndex].Type == CutsceneElementType.Special)
			{
				var special = Asset.Elements[CutSceneIndex].GetSpecial();
				if (Progress >= special.Time)
				{
					MoveUp();
					return;
				}
				Progress += Time.deltaTime * localSpeedModif;
			}
			else
			{
				var plain = Asset.Elements[CutSceneIndex].GetPlain();
				
				
				if (Time.time - startCutsceneTime < 0.4f )
					return;
				Progress += Time.deltaTime * localSpeedModif / timePerCharacter;

			
				var lastIndex = Math.Min(plain.Text.Length - 1, Mathf.CeilToInt(Progress) - 1);
				void MoveUntil(char endSymbol)
				{
					lastIndex++;
					while (plain.Text[lastIndex] != endSymbol)
					{
						lastIndex += 1;
					}
					Progress = lastIndex + 2;// one at the end
				}
				while (lastIndex< plain.Text.Length )
				{
					if (plain.Text[lastIndex] == '<')
					{
						MoveUntil('>');
						lastIndex++;
					}
					else if (plain.Text[lastIndex] == ':')
					{
						int start = lastIndex;
						MoveUntil('@');
						string flag = plain.Text.Substring(start+1, lastIndex - start-1);
						HandleTag(flag);
						lastIndex++;
					}
					else break;
					
				}

			
				textEntity.text =whipedText
					.Substring(0,   Math.Min(whipedText.Length, lastIndex+1));
				if (Progress >= plain.Text.Length)
				{
					if (Asset.Elements[CutSceneIndex].Interrupt)
						MoveUp();
				}
			}
		}
		[NaughtyAttributes.Button("Next", enabledMode: EButtonEnableMode.Playmode)]
		public bool Press()
		{
			if (!InProgress) return false;
			if (Asset.Elements[CutSceneIndex].Type == CutsceneElementType.Special) return false;
			var plain = Asset.Elements[CutSceneIndex].GetPlain();
			if (Progress >= plain.Text.Length)
			{
				if (MoveUp()) return true;
				return true;
			}
			Progress = plain.Text.Length;
			pauseTime = 0;
			return true;

		}
		private bool MoveUp()
		{
			CutSceneIndex++;
			Progress = 0;
			if (DisposeIfNeeded()) return true;	
			
			Prepare();
			return false;
		}
		bool DisposeIfNeeded()
		{
			if (CutSceneIndex >= Asset.Elements.Count)
			{
				Dispose();
				return true;
			}
			return false;
		}
		public void Dispose()
		{
			if (!InProgress) return;
			InProgress = false;
			if(musicSource)musicSource.Stop();
			OnCutsceneEnded(this, defaultAsset);

		}
		public void ClearAll()
		{
			if (InProgress)
			{
				throw new InvalidOperationException("this cannot be invoked when dialogue is in progress");
			}
			whoTextEntity.text = string.Empty;
			profileEntity.sprite = null;
			textEntity.text = string.Empty;
		}

		private void Prepare()
		{
			if (talkingSource != null)
				talkingSource.Stop();
			foreach (var element in visualElements)
			{
				UnityEngine.Object.Destroy(element);
			}
			visualElements.Clear();
			Asset.Elements[CutSceneIndex].StartEffect?.Play(this);
			specialTween?.Kill();

			mainBox.SetAllGraphicsAlpha(1);
			appearText.SetActive(false);
			localSpeedModif = 1;
			pauseTime = 0;
			if (Asset.Elements[CutSceneIndex].Type == CutsceneElementType.Special)
			{
				var special = Asset.Elements[CutSceneIndex].GetSpecial();
				appearText.GetComponent<Image>().color = special.Color;

				mainBox.SetAllGraphicsAlpha(0);
				textEntity.text = string.Empty;
				
				Sequence seq = DOTween.Sequence();
	
				if (special.HideTextBox)
				{
					foreach (var graphicElement in mainBox.GetComponentsInChildren<Graphic>())
					{

						seq.Insert(0, graphicElement.DOFade(0, special.Time / 3f));
						if (CutSceneIndex + 1 < Asset.Elements.Count)
						{
							var next = Asset.Elements[CutSceneIndex + 1].GetPlain();
							if (next != null)
							{
								seq.InsertCallback(special.Time / 3f, (() =>
								{
									profileEntity.sprite = next.Sprite;
									whoTextEntity.text = next.Who;
								}));
							}
						}
						seq.Insert(special.Time * (2 / 3f), graphicElement.DOFade(1, special.Time / 3f));

					}
				}
				if (!string.IsNullOrEmpty(special.ShowText))
				{
					appearText.SetActive(true);
					appearText.SetAllGraphicsAlpha(0);// could be optimize away ( this and all calls to it
					var showTextEntity = appearText.GetComponentInChildren<Text>();
					showTextEntity.text = special.ShowText;
					foreach (var graphicElement in appearText.GetComponentsInChildren<Graphic>())
					{


						if (!special.dontFadeIn)
							seq.Insert(0, graphicElement.DOFade(1, special.Time / 4f));
						else
							graphicElement.color = new Clr(graphicElement.color, 1);
						if(!special.dontFadeOut)
							seq.Insert(special.Time * (3f / 4), graphicElement.DOFade(0, special.Time / 4f));
						else
							seq.InsertCallback(special.Time ,()=> graphicElement.color= new Clr(graphicElement.color,1));
						if (special.changeBack)
							seq.InsertCallback(special.Time / 4f, (() =>
							{
								MainMenuHandler.Current.ForceBack(special.newBackValue);
							}));

					}
				}
				seq.SetTarget(mainBox).SetLink(this.gameObject);
				specialTween = seq;

			}
			else // plain
			{
				PlainDialogueElement plain = Asset.Elements[CutSceneIndex].GetPlain();

				if (plain.Clip != null&& talkingSource!=null)
				{
					talkingSource.clip = plain.Clip;
					talkingSource.Play();

				}
				whoTextEntity.text =plain.Who;
				whipedText=	Regex.Replace(plain.Text, @":.*?\@", m=>new string('​',m.Length));
				profileEntity.sprite = plain.Sprite;


				if (definition == null) return;
				CharacterAsset character= definition.Get(plain.Who.Trim());
				if (character != null)
				{
					int index = 0;
					foreach (VisualElementAsset element in character.Elements)
					{
						if (visualPoints.Count <= element.VisualPoint)
						{
							Debug.LogError($"Visual point nr {element.VisualPoint} was not found but was requested by {plain.Who}[{index}], " +
							               $"visual element will be skipped. Add it  ");
							continue;
						}
						Image image =
							Instantiate(visualImagePrefab,
								visualPoints[element.VisualPoint].Get2DPos(),
								Quaternion.identity, profileEntity.transform);
						image.sprite = element.Sprite;
						element.Movement.Activate(image.gameObject);
						visualElements.Add(image.gameObject);
						index++;
					}
				}
			}
			
		}


	}
}