#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace LetterBattle.Editor
{
    [CustomPropertyDrawer(typeof(CyberOptional<>))]
    public class CyberOptionalPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            const float BOOL_SPACE = 1f/15;
            const float VALUE_SPACE = 1-BOOL_SPACE;
            
            EditorGUI.BeginProperty(position, label, property);
            bool guiOnStart = GUI.enabled;
            var custom = property.FindPropertyRelative("custom");
            var value = property.FindPropertyRelative("value");

            Rect cur = new Rect(position)
            {
                width = VALUE_SPACE/3 * position.width
            };
            
            EditorGUI.LabelField(cur,label);
            cur.x += cur.width;
            cur.width = position.width * BOOL_SPACE;
            custom.boolValue= GUI.Toggle(cur, custom.boolValue,string.Empty);
            cur.x += cur.width;
            cur.width = position.width * VALUE_SPACE *(2/3f);

            if (custom.boolValue)
                EditorGUI.PropertyField(cur, value, GUIContent.none);
            else
            {
                EditorGUI.LabelField(cur,"null",new GUIStyle( EditorStyles.boldLabel));
            }
           
           
            
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif