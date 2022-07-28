using UnityEditor;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
	public class RequiredPropertyValidator : PropertyValidatorBase
	{
		public override void ValidateProperty(SerializedProperty property)
		{
			RequiredAttribute requiredAttribute = PropertyUtility.GetAttribute<RequiredAttribute>(property);

			if (property.propertyType == SerializedPropertyType.ObjectReference)
			{
				if (property.objectReferenceValue == null)
				{
					string errorMessage = property.name + " is required";
					if (!string.IsNullOrEmpty(requiredAttribute.Message))
					{
						errorMessage = requiredAttribute.Message;
					}
					Color bf =EditorStyles.helpBox.normal.textColor;
					EditorStyles.helpBox.normal.textColor = new Color(244/255f, 113/255f, 116/255f,1);
					NaughtyEditorGUI.HelpBox_Layout(errorMessage, MessageType.None, context: property.serializedObject.targetObject);
					EditorStyles.helpBox.normal.textColor = bf;
				}
			}
			else
			{
				//string warning = requiredAttribute.GetType().Name + " works only on reference types";
				//NaughtyEditorGUI.HelpBox_Layout(warning, MessageType.Warning, context: property.serializedObject.targetObject);
			}
		}
	}
}
