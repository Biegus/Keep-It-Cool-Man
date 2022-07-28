#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace LetterBattle.Editor
{
    [CustomEditor(typeof(LetterType))]
    public class LetterTypeEditor : UnityEditor.Editor
    {
        private SerializedProperty behaviourTypeProperty;
        private SerializedProperty prefabProperty;
        private BehaviorTable[] users = new BehaviorTable[0];
        private string nameForTable = string.Empty;
        private GUIStyle greenlabel;
        private const string PATH_TO_TABLES = "Assets/Resources/Content/Public/1Tables";

        private void OnEnable()
        {
            RefreshUsages();
            
            behaviourTypeProperty ??= this.serializedObject.FindProperty("behaviourType");
            prefabProperty ??= this.serializedObject.FindProperty("prefab");
        }
        public override void OnInspectorGUI()
        {
          
            serializedObject.Update();
            DrawFields();
            LetterEditorHelper.LayoutDoLines(2);
            DrawReferences();
            LetterEditorHelper.LayoutDoLines(1);
            DrawAddTableButton();
            serializedObject.ApplyModifiedProperties();
        }
        private void DrawAddTableButton()
        {

            EditorGUILayout.BeginHorizontal();
            if (string.IsNullOrEmpty(nameForTable))
            {
                nameForTable = "normal";
            }
          
            nameForTable = EditorGUILayout.TextField(nameForTable);
          
            
            if (GUILayout.Button("Add table"))
            {
                BehaviorTable table= ScriptableObject.CreateInstance<BehaviorTable>();
                typeof(BehaviorTable).GetField("type", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(table,target);
                AssetDatabase.CreateAsset(table,$"{PATH_TO_TABLES}/B_{target.name}_" +
                                                $"{nameForTable}.asset");
                AssetDatabase.SaveAssets();
                RefreshUsages();
                
            }
            EditorGUILayout.EndHorizontal();
        }
     
        private void DrawReferences()
        {
         
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Label("Behaviour tables");
            if (GUILayout.Button("Find References"))
            {
                RefreshUsages();
            }
            EditorGUILayout.EndHorizontal();
            foreach (var user in users)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(user, typeof(BehaviorTable), false);
                if (GUILayout.Button("X",GUILayout.Width(EditorStyles.label.CalcSize(new GUIContent("X")).x*2)))
                {
                    string file= AssetDatabase.GetAssetPath(user);
                    AssetDatabase.RemoveObjectFromAsset(user);
                    var indexOf = file.IndexOf('/');
                    string pathWithoutAssets = file.Substring(indexOf+1, file.Length - indexOf-1);
                    string finalPath = ($"{Application.dataPath}/{pathWithoutAssets}");
                    File.Delete(finalPath);
                    Debug.Log(finalPath);
                    AssetDatabase.SaveAssets();
                    RefreshUsages();

                }
                EditorGUILayout.EndHorizontal();;
            }
        }
        private void DrawFields()
        {
            EditorGUILayout.PropertyField(behaviourTypeProperty);
            EditorGUILayout.PropertyField(prefabProperty);
        }
        private void RefreshUsages()
        {
            users = Resources.LoadAll<BehaviorTable>(string.Empty).Where(item => item.Type== (LetterType) target)
                .ToArray();
        }
    }
}
#endif