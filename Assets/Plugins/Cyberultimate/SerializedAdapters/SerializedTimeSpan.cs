#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Cyberultimate.Unity
{
    /// <summary>
    /// Serializing <see cref="System.TimeSpan"/> version.
    /// </summary>
    [Serializable]
    [Obsolete]
    public struct SerializedTimeSpan
    {
       
        public enum Mode
        {
            Ms,
            S,
            M,
            H,
            D,
            Y,
        }
        [SerializeField] private float seconds;
      
        [SerializeField]private Mode mode;
        public float TotalSeconds => seconds;
        public TimeSpan TimeSpan => TimeSpan.FromSeconds(seconds);

    
        public SerializedTimeSpan(TimeSpan time)
        {
            seconds = (float)time.TotalSeconds;
            mode = Mode.S;
        }
        public SerializedTimeSpan(float seconds)
        {
            this.seconds = seconds;
            mode = Mode.S;
        }
       

        public static implicit operator SerializedTimeSpan(TimeSpan timeSpan)
        {
            return new SerializedTimeSpan(timeSpan);
        }
        public static implicit operator SerializedTimeSpan(float seconds)
        {
            return new SerializedTimeSpan(seconds);
        }
    }
#if UNITY_EDITOR
    namespace Editor
    {
        [CustomPropertyDrawer(typeof(SerializedTimeSpan))]
        public class SerializeTimeSpanDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {

                EditorGUI.BeginProperty(position, label, property);

                SerializedProperty modeProp = property.FindPropertyRelative("mode");
                SerializedProperty secondsProp = property.FindPropertyRelative("seconds");

                float GetMultiplayer(SerializedTimeSpan.Mode mode)
                {
                    switch (mode)
                    {
                        case SerializedTimeSpan.Mode.Ms: return 1f / 1000;
                        case SerializedTimeSpan.Mode.S: return 1;
                        case SerializedTimeSpan.Mode.M: return 60;
                        case SerializedTimeSpan.Mode.H: return 60 * 60;
                        case SerializedTimeSpan.Mode.D: return 60 * 60 * 24;
                        case SerializedTimeSpan.Mode.Y: return 60 * 60 * 24 * 365;

                        default: throw new ArgumentException("Unknown value");
                    }

                }

                float multiplayer = GetMultiplayer((SerializedTimeSpan.Mode)modeProp.intValue);

                position = EditorGUI.PrefixLabel(position, label);
                Rect floatFieldRect = position;
                floatFieldRect.width *= 0.7f;
                Rect dropdownPos = position;
                dropdownPos.width *= 0.3f;
                dropdownPos.position += new Vector2(floatFieldRect.width, 0);
                secondsProp.floatValue = EditorGUI.FloatField(floatFieldRect, secondsProp.floatValue / multiplayer) * multiplayer;
                if (EditorGUI.DropdownButton(dropdownPos, new GUIContent(((SerializedTimeSpan.Mode)modeProp.intValue).ToString()), FocusType.Passive))
                {
                    GenericMenu genericMenu = new GenericMenu();
                    void AddElement(SerializedTimeSpan.Mode mode)
                    {
                        genericMenu.AddItem(new GUIContent(mode.ToString()), false, (d) =>
                        {
                            modeProp.intValue = (int)(SerializedTimeSpan.Mode)d;
                            modeProp.serializedObject.ApplyModifiedProperties();
                        }, mode);
                    }
                    AddElement(SerializedTimeSpan.Mode.Ms);
                    AddElement(SerializedTimeSpan.Mode.S);
                    AddElement(SerializedTimeSpan.Mode.M);
                    AddElement(SerializedTimeSpan.Mode.H);
                    AddElement(SerializedTimeSpan.Mode.D);
                    AddElement(SerializedTimeSpan.Mode.Y);
                    genericMenu.ShowAsContext();

                }
                EditorGUI.EndProperty();


            }
        }
    }
   
#endif
}

