using UnityEngine;
namespace Cyberultimate
{
    public class GameStateHelper
    {
        public static GameState GetGameState()
        {
            if (Application.isPlaying) return GameState.PlayMode;
            else return GameState.Editor;
        }
    }
}