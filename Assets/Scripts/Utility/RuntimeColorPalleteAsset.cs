using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LetterBattle;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UObject = UnityEngine.Object;

using LetterBattle.Utility;


namespace LetterBattle
{
	public enum ColorType
	{
		Gray = 0,
		Danger = 1,
		KindaWhite = 2,
		Main = 3,
		Black = 4,
		Secondary = 5,
		Success = 6
	}

	[CreateAssetMenu(menuName = "ColorPalletAsset")]
	public class RuntimeColorPalleteAsset : ScriptableObject
	{
		private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic| BindingFlags.GetField;

		[SerializeField]
		private string palettePath = "Assets/Editor/mainPalet.colors";
		[SerializeField][NaughtyAttributes.ReadOnly]
		private Color[] colors = new Color[0];
		
		public  Color GetColor(ColorType type)
		{
			return colors[(int)type];
		}

		public string GetColorHex(ColorType type, bool hashtag = true)
		{
			return GetColor(type).GetColorHex(hashtag);
		}
#if UNITY_EDITOR
		[NaughtyAttributes.Button()]

		public void DumpFromEditorAsset()
		{
			
			//the colors needs to be dump to another asset since this one is editor only for someo reason
			
			//ColorPresetLibrary this is the type that actually   is INTERNAL, ( thanks unity not very cool)
			UnityEngine.ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(palettePath);

			Type assetType = obj.GetType();
			FieldInfo presetField= assetType.GetField("m_Presets", BINDING_FLAGS);
			IReadOnlyList<object> presets = (IReadOnlyList<object>)presetField.GetValue(obj);
			colors = new Color[presets.Count];
			Type colorPresetType = presets.GetType().GetGenericArguments()[0];
			FieldInfo colorOfPresetField = colorPresetType.GetField("m_Color", BINDING_FLAGS);
			
			for (int i = 0; i < presets.Count; i++)
			{
				colors[i] =(Color)colorOfPresetField.GetValue(presets[i]);
			}
		}		
		[NaughtyAttributes.Button()]
		public void Clear()
		{
			colors = new Color[0];
		}
#endif
			
	}
	
}

#if UNITY_EDITOR
[CustomEditor(typeof(RuntimeColorPalleteAsset))]
public class RuntimeColorPalleteAssetEditor : NaughtyAttributes.Editor.NaughtyInspector
{
	public override void OnInspectorGUI()
	{
		GetSerializedProperties(ref _serializedProperties);
		EditorGUILayout.BeginHorizontal();
		DrawButtons();
		EditorGUILayout.EndHorizontal();
		DrawSerializedProperties();
		DrawNonSerializedFields();
		DrawNativeProperties();
		
	
	}
}
	#endif