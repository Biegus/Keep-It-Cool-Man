#nullable enable
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cyberultimate.Unity
{
    [CommandContainer]
    public static class BaseCommands
    {
        [CyberCommand("b_load_scene_by_name",GameState.PlayMode,"name")]
        public static string LoadScene(string[] args)
        {
            SceneManager.LoadScene(args[0]);
            return string.Empty;
        }
        
        
    }
}