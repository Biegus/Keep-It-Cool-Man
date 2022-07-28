using System;
namespace Cyberultimate
{
    [Flags]
    public enum GameState
    {
        None=0,
        PlayMode=1<<0,
        Editor=1<<1,
        Both=PlayMode | Editor
        
    }
}