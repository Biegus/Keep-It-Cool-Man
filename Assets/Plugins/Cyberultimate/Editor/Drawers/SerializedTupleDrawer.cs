#if UNITY_EDITOR
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using UnityEditor;
using Cyberultimate.Unity;

namespace Cyberultimate.Editor
{
    [CustomPropertyDrawer(typeof(SerializedTuple<,>))]
    public class SerializedTupleDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var x = property.FindPropertyRelative("X");
            var y = property.FindPropertyRelative("Y");
            return EditorGUI.GetPropertyHeight(x) + EditorGUI.GetPropertyHeight(y) + EditorGUIUtility.singleLineHeight;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var x = property.FindPropertyRelative("X");
            var y = property.FindPropertyRelative("Y");

            GUIStyle style = new GUIStyle("label")
            {
                richText = true
            };
            float xHeight;
            label.text = $"<b>{label.text}</b>";
            EditorGUI.LabelField(new Rect(position) { height = EditorGUIUtility.singleLineHeight }, label,style);
            EditorGUI.indentLevel++;
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(new Rect(position) { height = xHeight = EditorGUI.GetPropertyHeight(x)},x,true);
            position.y += xHeight;
            EditorGUI.PropertyField(new Rect(position) { height = EditorGUI.GetPropertyHeight(y) }, y, true);
            EditorGUI.indentLevel--;

        }
    }
}
#endif