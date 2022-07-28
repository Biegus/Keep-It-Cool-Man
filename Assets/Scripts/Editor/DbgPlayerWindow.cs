#if UNITY_EDITOR
using System;
using System.Linq;
using Cyberultimate;
using LetterBattle.Utility;
using UnityEditor;
using UnityEngine;
namespace LetterBattle.Editor
{
    public class DbgPlayerWindow : EditorWindow
    {
        private int level;
        private bool autoPilotEnabled;
     
        private bool skipCutscene = false;
        [MenuItem("Kicm/DebugPlayerWindow")]
        private static void ShowWindow()
        {
            var window = GetWindow<DbgPlayerWindow>();
            window.titleContent = new GUIContent("Dbg Player");
            window.Show();
        }
        private void OnEnable()
        {
            level = EditorPrefs.GetInt($"{nameof(DbgPlayerWindow)}/level");
        }
        private void Update()
        {
            Repaint();
        }

        private void OnGUI()
        {
            autoPilotEnabled = EditorGUILayout.Toggle("Auto pilot", autoPilotEnabled);
            skipCutscene = EditorGUILayout.Toggle("Skip cutscene", skipCutscene);
            GUILayout.Label("Level:");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("<"))
                level--;
            if (GUILayout.Button(">"))
                level++;
            level = EditorGUILayout.IntField(level);
            
            level = Mathf.Clamp(level, 0, GameAsset.Current.Levels.Count - 1);
            
            LevelAsset asset= (LevelAsset)  EditorGUILayout.ObjectField(GameAsset.Current.Levels[level],typeof(LevelAsset),false);
            int? index = GameAsset.Current.Levels.Index(item => item == asset);
            level = index.Value;

            
            EditorGUILayout.EndHorizontal();
            if (Application.isPlaying == false)
                if (GUILayout.Button("Run"))
                {
                    if (ValidatorSystem.ValidateAndPrint(GameAsset.Current.Levels[level], string.Empty))
                        DebugLevelRunner.Run(level, autoPilotEnabled, skipCutscene);

                }
        }
        private void OnDisable()
        {
            EditorPrefs.SetInt( $"{nameof(DbgPlayerWindow)}/level",level);
        }
    }
}
#endif