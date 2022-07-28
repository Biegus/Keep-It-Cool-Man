using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using UnityEngine;

namespace LetterBattle
{
    public class SuperShySpawnBehaviour : ISideSpawnBehaviour
    {
        [SerializeField] private uint times = 4;
        [SerializeField][Range(0.1f,10)] private float distaneDifference = 0.5f;
        [SerializeField] [Range(0, 7f)] private float pushed = 0;
        
       
        public int PushEffect(in SpawnData data, ComponentsCache cache, SpawnBehavior owner)
        {
             float BASE_RADIUS = 10.5f-pushed;

             
            var obj = cache.GameObject;
            DirectionMover dirMover = cache.Get<DirectionMover>();
            RoundingMover roundingMover = cache.Get<RoundingMover>();
            Transform target = data.Target;
            bool goingIn = true;
            int i = 0;
            Tween tween=null;
            goingIn = true;
            tween= DoTweenHelper.DoUpdate(obj, () =>
            {
                float distanceSqrt = (target.Get2DPos() - obj.transform.Get2DPos()).sqrMagnitude;
                if (i >= times)
                {
                    tween.Kill();
                    return;
                } 
                if ((distanceSqrt < BASE_RADIUS-distaneDifference && goingIn) || (distanceSqrt > BASE_RADIUS + distaneDifference&& !goingIn))
                {
                    if (dirMover != null && dirMover.enabled)
                        dirMover.Direction = -dirMover.Direction;
                    else if (roundingMover != null && roundingMover.enabled)
                        roundingMover.ManualDecreasingSpeed = -roundingMover.ManualDecreasingSpeed;
                        goingIn = !goingIn;
                    i++;
                }
            });
            return 0;
        }
    }
}