#if UNITY_EDITOR
using System.Text;
using LetterBattle;
using LetterBattle.Utility;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
namespace Editor
{
    public class ValideBeforeBuildPreprocess:IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report)
        {
            GameAsset.ReLoad();
      
            StringBuilder details = new StringBuilder();
            
            var result = ValidatorSystem.Validate(GameAsset.Current,$"GameAsset");
            if (!result.IsOk)
            {
                details.AppendLine(result.GetDetails());
            }
            
            if (details.Length != 0)
                throw new BuildFailedException($"{ValidateResult.ERROR_PRE_HEADER}\n{details}");
        }
    }
}
#endif