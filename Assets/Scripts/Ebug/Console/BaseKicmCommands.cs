using System.IO;
using Cyberultimate;
using Cyberultimate.Unity;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace LetterBattle
{
    [CommandContainer]
    public static class BaseKicmCommands
    {
        
        [CyberCommand("show_save_file")]
        public static string GetFileRaw(string[] args)
        {
            using StreamReader reader = new StreamReader(new FileStream(SaveSystem.PATH, FileMode.Open));
            return reader.ReadToEnd();
        }
        
        [CyberCommand("remove_save_file")]
        public static void DeleteState(string[] args)
        {
            SaveSystem.Delete();
        }
        [CyberCommand("try_to_load_save_file")]
        public static void TryLoadState(string[] args)
        {
            GameManager.TryToLoad();
        }
        [CyberCommand("try_to_save_to_file")]
        public static void TrySaveState(string[] args)
        {
            GameManager.TryToSave();
        }
        
        [CyberCommand("force_game_start")]
        public static void GameStart(string[] args)
        {
            GameManager.StartTheGame();
        }
        [CyberCommand("force_game_stop")]
        public static void GameStop(string[] args)
        {
            GameManager.StopTheGame();
        }
        [CyberCommand("game_next_level")]
        public static void GameNextLevel(string[] args)
        {
            GameManager.FinalizeLevel(-1, true);
        }
        
        [CyberCommand("level_reset")]
        public static void LevelReset(string[] args)
        {
            GameManager.RestartLevel();
        }
        [CyberCommand("allow_for_debug_gui", GameState.PlayMode)]
        public static void AllowForDebugGUi(string[] args)
        {
            DebugGUI.ActiveMode = DebugGUI.Mode.HiddenWithButton;
        }
    }
}