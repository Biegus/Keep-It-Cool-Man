using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using UnityEngine;
namespace LetterBattle
{
    public class EnableRotationAfterSideSpawnBehaviour : ISideSpawnBehaviour
    {
        [NaughtyAttributes.MinMaxSlider(0, 4)]
        [SerializeField] private Vector2 time = new Vector2(2, 3);
      
        public int PushEffect(in SpawnData data, ComponentsCache cache, SpawnBehavior owner)
        {
            Vector2 pos = data.Target.Get2DPos();
            float finalTime = Randomer.Base.NextFloat(time.x, time.y);
            DOVirtual.DelayedCall(finalTime, () =>
            {
                var actionLetter = cache.Get<ActionLetter>();
                
                var directionMover = cache.Get<DirectionMover>();
                directionMover.enabled = false;
                var roundingMover = cache.Get<RoundingMover>();
                roundingMover.enabled = true;
               roundingMover.SetLinearAndRadiusToSimulateCertainPosition(cache.GameObject.transform.position, pos);
               roundingMover.AngleSpeed = directionMover.Speed / roundingMover.CurrentRadius;

            });
            return 0;
        }
    }
}