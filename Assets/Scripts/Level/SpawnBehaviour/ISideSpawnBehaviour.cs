using System;
using LetterBattle.Utility;
namespace LetterBattle
{
 
    public interface ISideSpawnBehaviour
    {
        int PushEffect(in SpawnData data, ComponentsCache cache, SpawnBehavior owner);
    }
}