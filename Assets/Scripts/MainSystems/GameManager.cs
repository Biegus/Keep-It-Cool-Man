using System;
using System.Linq;
using Cyberultimate.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace LetterBattle
{
	[CommandContainer]
	public static class GameManager
	{
		private static bool afterCutscene;
		
		[CommandProperty(get:true,set:false,name:"game_state")]
		public static PlayerState PlayerState { get; private set; }
		public static bool IsGameOn { get; private set; } = false;
		public static int CurrentLevelNumber { get; private set; } = -1;
		public static LevelAsset CurrentLevel { get; private set; }
		public static LevelAsset LevelOfLastGame { get; private set; } = null;
		public static int LevelNumberOfLastGame { get; private set; } = -1;
		public static AudioSource MusicSource { get; private set; }
		public static bool WaitingForCutscene { get; private set; }
	

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		public static void Init()
		{
			if (IsGameOn)
				StopTheGame();
			PlayerState = new PlayerState();
			TryToLoad();

		}
		
		public static bool TryToLoad()
		{
			PlayerState loaded = SaveSystem.Load();

			if (loaded == null)
			{
				Debug.Log("File save wasn't found");
				
				return false;
			}
			else
			{
				Debug.Log("Loaded save file");
				PlayerState = loaded;
				return true;
			}

			
		}
		public static void TryToSave()
		{
			SaveSystem.Save(PlayerState);
		}
		
		
		public static void StopTheGame(ScoreData score= new ScoreData())
		{
			SetScoreForLevel(CurrentLevelNumber, score);
			GameManager.TryToSave();
			LEvents.Base.OnGameEnded.Call(EventArgs.Empty);
			
			RemoveMusicSource();
			WaitingForCutscene = false;
			IsGameOn = false;
			LevelOfLastGame = CurrentLevel;
			LevelNumberOfLastGame = CurrentLevelNumber;
			CurrentLevelNumber = -1;
			CurrentLevel = null;


			SceneManager.LoadScene("Menu");
		}
		private static void RemoveMusicSource()
		{

			UnityEngine.Object.Destroy(MusicSource);
		}

		public static void StartTheGame(int levelToForce = -1, bool skipCutscene=false)
		{
			if (IsGameOn) StopTheGame();
			IsGameOn = true;
			if (levelToForce != -1)
				CurrentLevelNumber = levelToForce - 1;
			BuildAudioSource();
			FinalizeLevel( -1,false,skipCutscene);
		}
		private static void BuildAudioSource()
		{
			MusicSource = GameAsset.Current.AudioSourcePrefab
				? UnityEngine.Object.Instantiate(GameAsset.Current.AudioSourcePrefab)
				: new GameObject().AddComponent<AudioSource>();
			
			MusicSource.clip = GameAsset.Current.DefaultMusic;
			MusicSource.Play();
			UnityEngine.Object.DontDestroyOnLoad(MusicSource);
		}

		private static void ThrowIfWaitingForCutscene()
		{
			if (WaitingForCutscene)
			{
				throw new InvalidOperationException("Waiting for cutscene..");
			}
		}

		public static void FinalizeLevel(int levelToForce, bool forceUnlock, bool skipCutscene=false)
		{
			ThrowIfWaitingForCutscene();
			if (!IsGameOn) return;
			
			if (forceUnlock)
				Unlock(CurrentLevelNumber);
		
			if (levelToForce != -1)
				CurrentLevelNumber = levelToForce - 1;

			if (IsLevelLast(CurrentLevelNumber))
			{
				
				UnlockAchievementForChapter(GameAsset.Current.GetChapter(CurrentLevelNumber));
			}
			ICutsceneData cutscene=null;
			
			if (CurrentLevel?.AfterCutscene != null && forceUnlock)
			{
				cutscene = CurrentLevel.AfterCutscene;
			}
			Unlock(CurrentLevelNumber-1); // one level before always has to be unlocked
			
			GameManager.TryToSave();
			do
			{
				CurrentLevelNumber++;

				if (CurrentLevelNumber >= GameAsset.Current.Levels.Count)
				{
					//StopTheGame();
					CurrentLevel = null;
					break;
				}
				CurrentLevel = GameAsset.Current.Levels[CurrentLevelNumber];
			} while (CurrentLevel.Skip);

			if (cutscene != null)
			{
				
				RunCutscene(cutscene,true);
			}
			else if (CurrentLevel!=null &&  !skipCutscene &&CurrentLevel.PreCutscene != null)
			{

				RunCutscene(CurrentLevel.PreCutscene,false);
			}
			else
			{
				LoadLevel();
			}
			
		}
        
		private static void RunCutscene(ICutsceneData cutsceneAsset, bool after)
		{
			afterCutscene = after;
			MusicSource.Pause();
			SceneManager.LoadScene("Menu");
			MainMenuHandler.SavedLevelIndex = CurrentLevelNumber;
			MainMenuHandler.CutsceneAsset = cutsceneAsset;
			WaitingForCutscene = true;
		}

		public static void CallCutsceneEnded()
		{
			if (WaitingForCutscene)
			{
				if (CurrentLevel == null)
                {
					StopTheGame();
					MainMenuHandler.Current.LoadEndingScreen();
				}
				else if (afterCutscene && CurrentLevel.PreCutscene!=null)
				{
					RunCutscene(CurrentLevel.PreCutscene,false);
				}
				else
				{
					MusicSource.enabled = true;
					MusicSource.Play();
					WaitingForCutscene = false;
                    TransitionHandler.Current.DoTransition().OnComplete(() => LoadLevel());
                }
			}
 			else
				Debug.LogWarning("Game manager wasn't waiting for cutscene");
		}
        
		private static void LoadLevel()
		{
			if (CurrentLevel.CustomMusic != null)
			{
				MusicSource.clip = CurrentLevel.CustomMusic;
				MusicSource.Play();
			}
			else if (MusicSource.clip != GameAsset.Current.DefaultMusic)
			{
				MusicSource.clip = GameAsset.Current.DefaultMusic;
				MusicSource.Play();
			}
			

			SceneManager.LoadScene("ModelScene");
			SceneManager.LoadScene(CurrentLevel.SceneId, LoadSceneMode.Additive);
		}
        
		private static void Unlock(int level)
		{
			PlayerState.LevelUnlocked = Math.Max(PlayerState.LevelUnlocked, level + 1);
		}

		private static void UnlockAchievementForChapter(int chapter)
		{
			SteamHelper.Unlock($"STORYLINE_ACHIEVEMENT_{chapter}");
		}

		private static bool IsLevelLast(int level)
		{
			if (level == -1)
				return false;
			return GameAsset.Current.Chapters[GameAsset.Current.GetChapter(level)].Levels.Count-1 ==
				GameAsset.Current.FromRawToCertain(level).level;
		}
		public static bool SetScoreForLevel(int level,in ScoreData score)
		{
			
			if (score.Equals( ScoreData.None)|| level==-1) return false;
			Unlock(level);
			if (PlayerState.Scores.Count <= level)
			{
				PlayerState.Scores.AddRange(Enumerable.Range(0,level-PlayerState.Scores.Count +1).Select(item=>new ScoreData(-1,-1,RankLevel.Unknown)));
				PlayerState.Scores[PlayerState.Scores.Count - 1] = score;
			}
			else
			{
				PlayerState.Scores[level] = ScoreData.GetBetter(PlayerState.Scores[level],score);
			}
			
			bool res= PlayerState.Scores[level].Equals( score);

			CheckIfShouldGetAchievementForScore(level,score);
			return res;
		}

		private static void CheckIfShouldGetAchievementForScore(int level,in ScoreData score)
		{
			int chapter = GameAsset.Current.GetChapter(level);
			if (chapter > 0 && score.Rank == RankLevel.APlus) //not earth
			{
				SteamHelper.Unlock("BEAT_ACHIEVEMENT_1");//beat level other than earth with S rank
			}
			UnlockAAchievementIfShould();
			
		}

		private static void UnlockAAchievementIfShould()
		{
			if (GameAsset.Current.Levels.Count > GameManager.PlayerState.Scores.Count)
				return;
			for (int i = 0; i < GameAsset.Current.Levels.Count;i++)
			{
				if ( GameManager.PlayerState.Scores[i].Rank > RankLevel.B)
				{
					return;
				}
			}

			SteamHelper.Unlock("BEAT_ACHIEVEMENT_0");//beat all levels at least with A rank
		}
		public static void RestartLevel()
		{
			ThrowIfWaitingForCutscene();
			CurrentLevelNumber--;
			FinalizeLevel( -1,false,skipCutscene:true);
		}
	}
}
