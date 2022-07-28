#if UNITY_EDITOR
using System;
using System.Linq;
using LetterBattle;
using UnityEditor;
using UnityEngine;
namespace LetterBattle.Editor
{
    public class GameAssetHelper : EditorWindow
    {
        private AnimationCurve curve = new AnimationCurve();
        private float highestTime = 0;
        [MenuItem("Kicm/Helper")]
        private static void ShowWindow()
        {
            var window = GetWindow<GameAssetHelper>();
            window.titleContent = new GUIContent("Helper");
            window.Show();
        }
        private void OnEnable()
        {
            UpdateCurve();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUI.enabled = false;
            GUILayout.Label("Full game time");
          
            GUILayout.TextField(TimeSpan.FromSeconds( GameAsset.Current.GameSpan).ToString());
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            UpdateCurve();//should not be updated that much
            EditorGUILayout.CurveField(curve,Color.green, new Rect(0,0,GameAsset.Current.Levels.Count,highestTime));
        }

        private void UpdateCurve()
        {
            
            highestTime = 0;
            curve.keys = GameAsset.Current.Levels.Select((item, i) =>
            {
                var sum = item.Phases.Sum(item => item.Time);
                highestTime = Mathf.Max(highestTime, sum);
                return new Keyframe(i, sum);
            }).ToArray();
        }
        
    }
}
#endif