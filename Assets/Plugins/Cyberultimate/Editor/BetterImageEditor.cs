#if UNITY_EDITOR
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace Cyberultimate.Unity
{
    [CustomEditor(typeof(BetterImage))]
    public class BetterImageEditor: ImageEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SerializedProperty aspect = serializedObject.FindProperty("preserveAspect");
            EditorGUILayout.PropertyField(aspect, new GUIContent( aspect.displayName));
            serializedObject.ApplyModifiedProperties();

        }
    }
}
#endif