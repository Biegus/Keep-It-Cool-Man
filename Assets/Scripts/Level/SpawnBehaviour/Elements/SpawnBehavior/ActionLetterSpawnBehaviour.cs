using Cyberultimate.Unity;
using UnityEngine;
namespace LetterBattle
{
    public class ActionLetterSpawnBehaviour : KeyboardRelatedSpawnBehaviour
    {
        
        protected override DoneSpawnData InternalSpawn(in SpawnData data)
        {
            DoneSpawnData obj = base.InternalSpawn(in data);
            ActionLetter letter = cache.Get<ActionLetter>();
            letter.SetSide(data.Side);
            letter.SetLetter(Randomer.Base.NextRandomElement(GetRawLetters(data.CustomLetters,data.Side)));
            return obj;
        }
       
    }
}