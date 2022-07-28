using Cyberultimate.Unity;
using LetterBattle.Utility;
using UnityEngine;
namespace LetterBattle
{
    public class ForceOneDirectionSideBehaviour : ISideSpawnBehaviour
    {
        [SerializeField] private Direction direction;

        public int PushEffect(in SpawnData data, ComponentsCache cache, SpawnBehavior owner)
        {
            cache.Get<ActionLetter>()
                .DirectionMover.Direction =- data.Side.ToDirection();
            return 0;
        }
    }
}