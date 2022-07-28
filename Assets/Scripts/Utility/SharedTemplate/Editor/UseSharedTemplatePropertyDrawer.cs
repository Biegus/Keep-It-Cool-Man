#if UNITY_EDITOR
using Cyberultimate;
using Cyberultimate.Editor;
using UnityEditor;
using UnityEngine;
using  LetterBattle.Utility;
using  LetterBattle.Utility.SharedTemplate;

namespace LetterBattle.Utility.SharedTemplate
{
    [CustomPropertyDrawer(typeof(UseSharedTemplateAttribute))]
    public class UseSharedTemplatePropertyDrawer : PropertyDrawer
    {
        private const int X_SPACE = 20;

        
        private bool IsNotPrimitive(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Generic;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (IsNotPrimitive(property)) return  EditorGUI.GetPropertyHeight(property)+  base.GetPropertyHeight(property, label)*2; // could be cached
            return EditorGUI.GetPropertyHeight(property);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool disable = false;
            if (IsNotPrimitive(property))
            {
                disable = true;
                Rect boxPos = position;
                boxPos.height=EditorGUIUtility.singleLineHeight * 2;
                position.y += boxPos.height;
                EditorGUI.HelpBox(boxPos, $"SharedTemplates can only be used on primitive types. \"{fieldInfo.FieldType}\" is not one of them.",MessageType.Error);
            }
            var atr = (UseSharedTemplateAttribute)attribute;
            Rect baseRect = position;
            baseRect.width -= X_SPACE * 2;
            EditorGUI.PropertyField(baseRect, property, label,true);

            position.height = EditorGUIUtility.singleLineHeight;
            position.x += baseRect.width;
            position.width = X_SPACE;

            if (!disable)
            {
                if (EditorGUI.DropdownButton(position, new GUIContent("?"), FocusType.Keyboard))
                {

                    GenericMenu genericMenu = new GenericMenu();
               
                    foreach (var template in TemplateAssetHandler.GeneralTemplateAsset.Get(atr.GroupName))
                    {
                        var cache = template;
                        genericMenu.AddItem(new GUIContent(template.Name), false, () =>
                        {
                            property.SetValue(cache.Value);
                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }
                    genericMenu.ShowAsContext();
                }

                position.x += X_SPACE;

                if (GUI.Button(position, new GUIContent("+")))
                {
                    AddTemplateWindow.ShowWindow(atr.GroupName,property.GetValue());
                }
            }

           
        }




    }
}
#endif