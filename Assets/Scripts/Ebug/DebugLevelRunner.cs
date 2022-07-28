#if UNITY_EDITOR

using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using Cyberultimate;
using LetterBattle.Utility;
using UnityEngine.SceneManagement;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LetterBattle
{
    // now i know about editor prefs that whole lame fun with text files seems kinda lame, thing that could be made better
    //TODO: change to editor prefs instead of raw txt file
    
    public static class DebugLevelRunner
    {
        private static bool resetSceneEventFinished = false;
        private static readonly string menuPath = "Assets/Resources/Scenes/Menu.unity";
        public static void StartByGameAsset( LevelAsset asset)
        {
            GameAsset.ReLoad();
            int? index = GameAsset.Current.GetIndexOflevel(asset);
            if (index == null)
            {
                UnityEngine.Debug.Log($"{asset.name} must be set in game asset");
                return;
            }
            DebugLevelRunner.Run(index.Value,false);
        }
        public static void StartByGameAssetWithValidation(LevelAsset asset)
        {
            
            var validation = ValidatorSystem.Validate(asset);
            if (validation.IsOk)
                DebugLevelRunner.StartByGameAsset(asset);
            else
                Debug.LogError($"{validation}");   
            
        }

        [MenuItem("Kicm/Start")]
        public static void JustStart()
        {
            Run(0,false);
        }
        private const string FORCED_TXT = "__forced.txt";
        private const string FORCED_LAST_SCENE_TXT = "__forced2.txt";
        public static void Run(int index,bool autoPilot, bool skipCutscene=false)
        {
            
            using StreamWriter sceneWriter = new StreamWriter(FORCED_LAST_SCENE_TXT);
            sceneWriter.WriteLine(SceneManager.GetActiveScene().path);
            UnityEditor.EditorApplication.isPlaying = true;
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            EditorSceneManager.OpenScene(menuPath);
            EditorApplication.isPlaying = true;
            //communication with play mode here is a bit of annoying, any other way fails
            //note added later: EditorPrefs, i didn't know about it lol also i guess they were other better solution i was just lazy
         
            using StreamWriter writer = new StreamWriter(FORCED_TXT);
            writer.WriteLine(index);
            writer.WriteLine(autoPilot);
            writer.WriteLine(skipCutscene);
            
        }
       
        [UnityEditor.InitializeOnLoadMethod]
        public static void PrepareResetSceneEvent()
        {
            if (resetSceneEventFinished) return;
            resetSceneEventFinished = true;
            EditorApplication.playModeStateChanged += ResetSceneEvent;
        }

        public static void ResetSceneEvent(PlayModeStateChange change)
        {
            if(change == PlayModeStateChange.EnteredEditMode)
                if(File.Exists(FORCED_LAST_SCENE_TXT))
                {
                    using (StreamReader reader = new StreamReader(FORCED_LAST_SCENE_TXT))
                    {
                        EditorSceneManager.OpenScene(reader.ReadToEnd().Trim());
                    }
                    File.Delete(FORCED_LAST_SCENE_TXT);
                }
        }
        
        public static int ReadAndStart(bool delete)
        {
            EditorSceneManager.playModeStartScene = null;
            if (!File.Exists(FORCED_TXT)) return -1;
            int level;
            int phase;
            using (StreamReader reader = new StreamReader(FORCED_TXT))
            {
                level= int.Parse(reader.ReadLine());
                bool auto = bool.Parse(reader.ReadLine());
                bool cutsceneSkip = bool.Parse(reader.ReadLine());
                if (auto)
                    AutoPilot.InvertState();
                GameManager.StartTheGame(level);
            }
          
            if(delete)     File.Delete(FORCED_TXT);
            return level;
        }
        
    }
}

#endif