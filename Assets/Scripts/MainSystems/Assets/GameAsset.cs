using Cyberultimate;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using LetterBattle.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
namespace LetterBattle
{
	[Serializable]
	public struct RankTextData
	{
		public string Name;
		[TextArea]
		public string Description;
		public Color Color;
	}
	[CreateAssetMenu(menuName = "GameAsset")]
	public class GameAsset : ScriptableObject, ISerializationCallbackReceiver
	{
		
		
		private static Lazy<GameAsset> lazyAsset = new Lazy<GameAsset>(() =>
		{
			#if UNITY_EDITOR
			if (!Application.isPlaying)
				return AssetDatabase.LoadAssetAtPath<GameAsset>("Assets/Resources/AMain/GameAsset.asset");
			#endif
			return Resources.Load<GameAsset>("AMain/GameAsset");
		});
		public static GameAsset Current => lazyAsset.Value;

		[SerializeField]
		[Expandable][ValidateNotNull]
		private ChapterAsset[] chapters = new ChapterAsset[0];
		
		[SerializeField] private Keyboard keyboard = new Keyboard("qwertyuiopasdfghjklzxcvbnm");
		[SerializeField] private float dmgForWrong = 0.33f;
		
		[ValidateNotNull][SerializeField] private RuntimeColorPalleteAsset pallete;

		[Header("Sounds")]
		[SerializeField] private AudioSource audioSourcePrefab = null;
		[SerializeField] private AudioClip defaultMusic = null;
		[SerializeField] private AudioClip dmgSound = null;
		[SerializeField] private AudioClip wrongLetterSound = null;
		[SerializeField] private AudioClip deathSound = null;
		[SerializeField] private AudioClip zoneEnterSound = null;// i don't like that idea
		[SerializeField] private AudioClip comboSound = null;
		[SerializeField] private AudioClip destructLetterSound = null;
		[SerializeField] public AudioClip levelFinishedAudio;
		
		[FormerlySerializedAs("audioMapAsset")] [NaughtyAttributes.Expandable] [SerializeField] private AudioMapAsset correctSounds = null;
		
		[Header("Score")]
		[SerializeField]
		private int scoreForShot = 10;
		[SerializeField]
		private int punishForNotFound = -2;
		[SerializeField]
		private int punishForGettingHit = -6;
		[SerializeField]
		private RankTextData[] textsForRanks = new RankTextData[6];
		
		[SerializeField] private  LineRenderer lineRenderConnectionPrefab;

			
		public RuntimeColorPalleteAsset Pallete => pallete;
		public Keyboard Keyboard => keyboard;
		public IReadOnlyList<ChapterAsset> Chapters => chapters;
		public IReadOnlyList<LevelAsset> Levels
		{
			get	
			{
				if(levelsRaw==null) InitGameAsset();
				return levelsRaw;
			}
		}
		public AudioMapAsset CorrectSounds => correctSounds;
		public float DmgForWrong => dmgForWrong;
		public AudioSource AudioSourcePrefab => audioSourcePrefab;
		public AudioClip DefaultMusic => defaultMusic;
		public AudioClip DmgSound => dmgSound;
		public AudioClip DeathSound => deathSound;
		public AudioClip WrongLetterSound => wrongLetterSound;
		public AudioClip ZoneEnterSound => zoneEnterSound;
		public AudioClip ComboSound => comboSound;
		public AudioClip LevelFinishedAudio => levelFinishedAudio;
		public AudioClip DestructLetterSound => destructLetterSound;
		public int ScoreForShot => scoreForShot;
		public int PunishForNotFound => punishForNotFound;
		public int PunishForGettingHit => punishForGettingHit;

		public IReadOnlyList<RankTextData> TextsForRanks => textsForRanks;

		private LevelAsset[] levelsRaw;
		private Dictionary<LevelAsset, (int chapter, int index)> levelLookup = null;
		public IReadOnlyDictionary<LevelAsset, (int chapter, int index)> LevelLookup => levelLookup;
		private int[] chapterSums = null;
		
		public float GameSpan
		{
			get
			{
				return chapters.Sum(item => item.Levels?.Sum(item => item.Phases.Sum(phase => phase.Time) + 2f) ?? 0);
			}
		}
		public LineRenderer LineRenderConnectionPrefab => lineRenderConnectionPrefab;
		public int FromCertainToRaw(int chapter, int level)
		{
			if (chapter < 0) throw new ArgumentException("Chapter has to be above 0");

			if (chapter == 0) return level;
			return chapterSums[chapter - 1] + level;
		}

		public (int chapter, int level) FromRawToCertain(int level)
		{
			int chapter = GetChapter(level);
			if (chapter == 0)
				return (0, level);

			return (chapter, level - chapterSums[chapter - 1]);
		}
		public int? GetIndexOflevel(LevelAsset asset)
		{
			int? index = this.Levels.Index(item => item == asset);
			return index;
		}
		public void InitGameAsset()
		{
			
			levelsRaw = Chapters.SelectMany(item => item.Levels).ToArray();
			chapterSums = new int[chapters.Length];
			chapterSums[0] = chapters[0].Levels.Count;
			for (int i = 1; i < chapters.Length; i++)
			{
				chapterSums[i] = chapterSums[i - 1] + chapters[i].Levels.Count;
			}

			var linq = Chapters.Select((ch, i) => ch.Levels.Select((item, j) => new
			{
				i,
				j,
				obj = item
			})).SelectMany(item => item);

			HashSet<LevelAsset> set = new HashSet<LevelAsset>();
			foreach (var el in linq)
			{
				if (set.Contains(el.obj))
				{
					Debug.LogError($"{el.obj} is added twice, this is not allowed");
					levelLookup = null;
					return;
				}
				set.Add(el.obj);
			}
			
			levelLookup = linq.ToDictionary(item => item.obj, item => (item.i, item.j));	
		}
        
		public int GetChapter(int levelId)
		{
			int chapterNumber;
			if (levelId >= Levels.Count)
			{
				chapterNumber = chapters.Length - 1;
			}
			else
				chapterNumber = LevelLookup[Levels[levelId]].chapter;

			return chapterNumber;
		}

		[RuntimeInitializeOnLoadMethod]
		public static void ReLoad()
		{
			GameAsset.Current.InitGameAsset();
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			
		}
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (this.textsForRanks.Length != 6)
			{
				var old = this.textsForRanks;
				this.textsForRanks = new RankTextData[6];
				if (old.Length > 6)
					Array.Copy(old, this.textsForRanks, 6);
				else
					Array.Copy(old,this.textsForRanks,5);
			}
		}
	}
}