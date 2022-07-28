using Cyberultimate;
using Cyberultimate.Unity;
using UnityEngine;
namespace LetterBattle
{
    public class BaseMoverBehaviour : ActionLetterSpawnBehaviour
    {
        [SerializeField] [Range(0.2f, 10)] protected float speed = 1;
  
        protected override DoneSpawnData InternalSpawn(in SpawnData data)
        {
            var obj = base.InternalSpawn(data);
            var mover = cache.Get<DirectionMover>();
            mover.Direction = data.Direction;
            mover.SpeedPlain = speed;
            
            return obj;

        }
    }
}