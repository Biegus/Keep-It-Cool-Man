#if UNITY_EDITOR
using System;
using System.Linq;
using System.Text;

using UnityEditor;
using UnityEditor.Graphs;

using UnityEngine;
namespace Cyberultimate.Unity.Editor
{
    public class EditorCyberConsoleWindow : EditorWindow
    {
        private const string USE_UNITY_CONSOLE = "Use Unity Console";
        private const string CLEAR = "Clear";
        private const string Run = "Run";
        private const string UNITY_CONSOLE_PREFIX = "Cyber console: ";
        private const string OUTPUT = "Output";

        //TODO: enable or disable showing of date and the command itself
        private string input = string.Empty;
        private string output = string.Empty;// TODO: handle it betty, now it can break or work inefficient for long strings
     
        private Vector2 scroll;
        private bool useUnityConsole = false;
        private  CyberCommand[] tips= new CyberCommand[0];
        private string cacheTipsText = string.Empty;
        private GUIStyle areaStyle;
        private bool enable; 
        
        [MenuItem("Cyberultimate/Cyber Console")]
        private static void ShowWindow()
        {
            var window = GetWindow<EditorCyberConsoleWindow>();
            window.titleContent = new GUIContent("Console");
            window.Show();
        }
        
        private void OnEnable()
        {
            if (enable)
                GenerateTips();
        }
        private void OnGUI()
        {
            if (GUILayout.Button(enable?"Disable":"Enable"))
            {
                enable = !enable;
                GenerateTips();
            }
            if (!enable) return;

            areaStyle = new GUIStyle("textarea")
            {
                richText = true
            };

            DrawSettings();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            DrawInputArea();
            DrawTips();
            DrawOutputArea();
            
        }
        private void DrawOutputArea()
        {

            EditorGUILayout.LabelField(OUTPUT);
            scroll = EditorGUILayout.BeginScrollView(scroll);
            GUILayout.TextArea(output.ToString(), areaStyle);
            EditorGUILayout.EndScrollView();
        }
        private void DrawTips()
        {

            EditorGUILayout.TextArea(cacheTipsText, areaStyle);
            if (GUILayout.Button(CLEAR))
            {
                output = string.Empty;
            }
        }
        private void DrawInputArea()
        {

            EditorGUILayout.BeginHorizontal();
            string nwInput = EditorGUILayout.TextField(input);
            bool changed = nwInput != input;
            input = nwInput;
            if (GUILayout.Button(Run))
            {
                Call();
            }
            if (changed)
            {
                GenerateTips();
            }
            EditorGUILayout.EndHorizontal();
        }
        private void DrawSettings()
        {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(USE_UNITY_CONSOLE);
            useUnityConsole = EditorGUILayout.Toggle(useUnityConsole);

            EditorGUILayout.EndHorizontal();
            ;
        }
        private void Call()
        {

            string response = CyberCommandSystem.CallCommand(input);
            if (useUnityConsole)
                Debug.Log($"{UNITY_CONSOLE_PREFIX} {response}");
            output = (response) + output;
        }
        public void RefreshTipCache()
        {
            cacheTipsText = tips.Aggregate(new StringBuilder(), (b, v) =>
            {
                bool wrongState = (GameStateHelper.GetGameState() & v.MetaData.GameState) == GameState.None;
                b.Append(v.ToString());
                if (wrongState) b.Append($"<color=Red> [{v.MetaData.GameState}]</color>");
                b.AppendLine();
                return b;
            }).ToString();
        }
        private void GenerateTips()
        {

            tips = CyberCommandSystem.GetTips(input??string.Empty).ToArray();
            RefreshTipCache();
        }
       
    }
}
#endif