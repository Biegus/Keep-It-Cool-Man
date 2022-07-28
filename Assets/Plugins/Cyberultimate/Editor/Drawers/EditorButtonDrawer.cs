#if UNITY_EDITOR
#nullable enable

using System.Reflection;
using Cyberultimate.Unity;
using UnityEditor;
using UnityEngine;

namespace Cyberultimate.Editor
{
    [CustomPropertyDrawer(typeof(InspectorButtonInformationAttribute))]
    public class EditorButtonDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var info = (InspectorButtonInformationAttribute)attribute;
            return info.Size switch
            {
                InspectorButtonSize.Normal => EditorGUIUtility.singleLineHeight,
                InspectorButtonSize.Medium => EditorGUIUtility.singleLineHeight * 2,
                InspectorButtonSize.Thick => EditorGUIUtility.singleLineHeight * 3,
            };
        }

        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {

            EditorGUI.BeginProperty(position, null, property);
            
            InspectorButtonInformationAttribute info= ((InspectorButtonInformationAttribute)attribute);
            var input = property.FindPropertyRelative("input");
            bool result;

            bool hasArgs = input.type != nameof(NoneType);
            if(hasArgs)
            {
                position.width /= 2;
                EditorGUI.PropertyField(new Rect(position){height=EditorGUIUtility.singleLineHeight}, input,
                    ((!string.IsNullOrEmpty( info.ArgumentDescription))? new GUIContent(info.ArgumentDescription): GUIContent.none));
                position.x += position.width;

            }

            bool bf = GUI.enabled;

            var isActive =(!(property.serializedObject.targetObject is  MonoBehaviour)) || (((MonoBehaviour)property.serializedObject.targetObject)).isActiveAndEnabled;
            if ((info.Limit.HasFlag(InspectorButtonLimits.NotInEditorMode) && !Application.isPlaying)
                || (info.Limit.HasFlag(InspectorButtonLimits.NotInPlayMode)&& Application.isPlaying)
                || (info.Limit.HasFlag(InspectorButtonLimits.NotIfObjectIsEnabled)&& isActive)
                || (info.Limit.HasFlag(InspectorButtonLimits.NotIfObjectIsNotEnabled) && !isActive))
            {
                GUI.enabled = false;
            }
            
            if (GUI.Button(position,info.Name))
            {
                MethodInfo? realMethod = property.serializedObject.targetObject.GetType()
                    .GetMethod(info.Method,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                if (realMethod == null)
                {
                    Debug.Log($"Method {info.Method} does not exist");
                }
                else
                {
                    realMethod.Invoke(property.serializedObject.targetObject, hasArgs ? new object[] { input.GetValue() } : null);
                }
            }

            GUI.enabled = bf;
            EditorGUI.EndProperty();

        }
    }
}
#endif