using UnityEngine;
namespace LetterBattle.Utility
{
    public static class ColorHelper
    {
        public static string GetColorHex(this Color color, bool hashtag=false)
        {
            string hex = "";

            if (hashtag)
            {
                hex += '#';
            }

            hex += $"{ColorUtility.ToHtmlStringRGBA(color)}";

            return hex;
        }
        
    }
}