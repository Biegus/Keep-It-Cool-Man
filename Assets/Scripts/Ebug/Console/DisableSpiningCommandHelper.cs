using Cyberultimate;
using Cyberultimate.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace LetterBattle
{
    [CommandContainer]
    public class DisableSpiningCommandHelper
    {

        private static bool disabled = false;
        [CyberCommand("disable_rotating",GameState.PlayMode)]
        public static string DisableRotating(string[] args)
        {

            disabled = !disabled;
            InternalChange();
            if (disabled)
            {
             
                SceneManager.sceneLoaded +=OnSceneManagerOnsceneLoaded;
            }
            else
            {
                SceneManager.sceneLoaded -=OnSceneManagerOnsceneLoaded;
            }
        
            return disabled.ToString();

        }
        private static void OnSceneManagerOnsceneLoaded(Scene s, LoadSceneMode mode)
        {
            if (s.name == "ModelScene") InternalChange();
        }

        private static void InternalChange()
        {
            GameObject.FindWithTag("Background").GetComponent<SpaceSpinner>().enabled = !disabled;

        }
    }
}