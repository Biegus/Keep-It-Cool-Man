using LetterBattle.Utility;
using NaughtyAttributes;
using UnityEngine;

namespace LetterBattle
{
    public class SetHpSideSpawnBehaviour : ISideSpawnBehaviour
    {
        [SerializeField][MinValue(0)] private uint Hp = 1;
        public int PushEffect(in SpawnData data, ComponentsCache cache, SpawnBehavior owner)
        {
            cache.Get<ActionLetter>().Hp.SetValue(Hp);
            return (int)Hp - 1;
        }
    }
}