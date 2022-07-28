#if UNITY_EDITOR
using LetterBattle.Utility;
using UnityEditor;
using UnityEngine;
namespace LetterBattle.Editor
{
    [CustomPropertyDrawer(typeof(SReadonlyArray<>))]
    public class SReadonlyArrayDrawer : PropertyDrawer
    {
        private const string ARRAY_NAME = "array";
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position,property.FindPropertyRelative(ARRAY_NAME),label,true);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(ARRAY_NAME));
        }
    }
}
#endif