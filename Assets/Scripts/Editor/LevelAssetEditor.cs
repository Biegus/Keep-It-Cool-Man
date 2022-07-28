#if UNITY_EDITOR
using System;
using System.Linq;
using Cyberultimate.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace LetterBattle.Editor
{
   
    [CustomEditor(typeof(LevelAsset))]
    public class LevelAssetEditor : UnityEditor.Editor
    {
        private LevelAssetSoftDrawer drawer;
        private void OnEnable()
        {
            drawer = new LevelAssetSoftDrawer(serializedObject, (LevelAsset) target);
        }
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Global Level Editor"))
            {
                GlobalLevelEditorWindow.ShowWindow();
            }
            
            drawer.OnInspectorGUI();
          
        }
    }
}
#endif