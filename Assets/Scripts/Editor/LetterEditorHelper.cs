#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace LetterBattle.Editor
{
    public static class LetterEditorHelper
    {
        
        public static void LayoutDoLines(int times)
        {
            GUILayoutUtility.GetRect(6f, EditorGUIUtility.singleLineHeight * times);
        }
    }
}
#endif