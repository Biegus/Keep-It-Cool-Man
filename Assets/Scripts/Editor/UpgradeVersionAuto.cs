#if UNITY_EDITOR
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace LetterBattle
{
   
   
    class LetterBattleBuildHelper : IPreprocessBuildWithReport
    {

        private Regex versionRegex = new Regex( @"\d+");

        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report)
        {
            string currentVersion = PlayerSettings.bundleVersion;
            PlayerSettings.bundleVersion =
                versionRegex.Replace(currentVersion, m => (int.Parse(m.Value) + 1).ToString());
        }
    }
 
}
#endif