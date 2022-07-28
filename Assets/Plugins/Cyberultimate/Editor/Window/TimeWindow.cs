#if UNITY_EDITOR
#nullable enable
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Cyberultimate.Editor
{ 
    public sealed class TimeWindow : EditorWindow
    {
        private Vector2 scroll = default;
        
        [MenuItem("Cyberultimate/Time")]
        public static void Open()
        {
            EditorWindow.GetWindow<TimeWindow>("Time").Show();
        }
        private void OnInspectorUpdate()
        {
            Repaint();
        }
        private void OnGUI()
        {
           
            if(Application.isPlaying==false)
            {
                EditorGUILayout.HelpBox("It's available only in play mode", UnityEditor.MessageType.Info);
                return;
            }
            scroll = EditorGUILayout.BeginScrollView(scroll,false,true);
            Time.timeScale = EditorGUILayout.FloatField("Time Scale", Time.timeScale);
            Application.targetFrameRate = EditorGUILayout.IntField("Max Frame Rate", Application.targetFrameRate);
            GUI.enabled=false;
            MakeField("Time", Time.time);
            MakeField("Unscaled Time", Time.unscaledTime);
            MakeField("Frame Count", Time.frameCount);
            MakeField("DeltaTime", Time.deltaTime);
            MakeField("Capture Framerate", Time.unscaledDeltaTime);
            MakeField("Fixed Delta Time", Time.fixedDeltaTime);
            MakeField("Fixed Unscaled DeltaTime", Time.fixedUnscaledDeltaTime);
            MakeField("Capture Delta Time", Time.captureDeltaTime);
            MakeField("Rendered Frame Count", Time.renderedFrameCount);
            MakeField("Capture Framerate", Time.captureFramerate);
            GUI.enabled = true;
            EditorGUILayout.EndScrollView();

        }
        private void MakeField(string name, float val)
        {
            EditorGUILayout.FloatField(name, val);

        }
    }


}

#endif