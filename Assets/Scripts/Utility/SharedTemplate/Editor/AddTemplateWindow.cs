#if UNITY_EDITOR
using System;
using Cyberultimate.Unity;
using UnityEditor;
using UnityEngine;
namespace LetterBattle.Utility.SharedTemplate
{
    public class AddTemplateWindow : EditorWindow
    {
        private string groupName;
        private object value;
        private string nameFromUser;
    
        public static void ShowWindow(string groupName, object value)
        {
            
            var window = GetWindow<AddTemplateWindow>();
            window.groupName = groupName;
            window.value = value;
            window.titleContent = new GUIContent("Add");
            window.minSize = window.minSize.CY(EditorGUIUtility.singleLineHeight * 3);
            window.position = new Rect(window.position)
            {
                position = GUIUtility.GUIToScreenPoint(Event.current.mousePosition),
                height = EditorGUIUtility.singleLineHeight * 3
            };
            window.ShowPopup();
         
        }
      
        private void OnGUI()
        {
            EditorGUILayout.LabelField(value?.ToString()?? "null");
            nameFromUser= EditorGUILayout.TextField("Name",nameFromUser);
            if (GUILayout.Button("+"))
            {
                TemplateAssetHandler.GeneralTemplateAsset.Add(groupName,value,nameFromUser);
                this.Close();
            }
        }
    }
}
#endif