#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace LetterBattle.Utility.SharedTemplate
{
    public static class TemplateAssetHandler
    {
        private static TemplatesAsset lazyAsset;
        public static TemplatesAsset GeneralTemplateAsset
        {
            get
            {
                if(lazyAsset==null) lazyAsset= AssetDatabase.LoadAssetAtPath<TemplatesAsset>("Assets/Templates.asset");
                if (lazyAsset == null)
                {
                    Debug.LogWarning("There's' no base templates asset");
                }
                return lazyAsset;
            }
           
        }
    }
}

#endif