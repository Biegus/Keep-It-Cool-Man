using Cyberultimate.Unity;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
namespace LetterBattle
{
    public class ReverseRoundingSpawnBehaviour : DirectionActionLetterSpawnBehavior
    {
        [SerializeField][NaughtyAttributes.MinValue(0)] private ParticleSystem.MinMaxCurve angleSpeed = Mathf.PI;
        [SerializeField] [NaughtyAttributes.MinMaxSlider(0,4)] private Vector2 delay = Vector2.zero;
        protected override DoneSpawnData InternalSpawn(in SpawnData data)
        {
            DoneSpawnData doneSpawnData= base.InternalSpawn(in data);
            ActionLetter actionLetter = cache.Get<ActionLetter>();
            Transform target = data.Target;
            DirectionMover mover = cache.Get<DirectionMover>();// need to cache, before lambda it could CHANGE
            var flickering = doneSpawnData.Obj.GetComponent<FlickingManager>();
            float currentDelay = Randomer.Base.NextFloat(delay.x, delay.y);
            if (flickering != null)
                flickering.Flick(currentDelay);
         
            DOVirtual.DelayedCall(  currentDelay, () =>
            {
                mover.enabled = false;
                doneSpawnData.Obj.transform.DOScale(doneSpawnData.Obj.transform.localScale * 2, 0.2f)
                    .SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() =>
                    {

                        RoundingMover roundingMover = doneSpawnData.Obj.AddComponent<RoundingMover>();
                        roundingMover.AngleSpeed = angleSpeed.Evaluate(0);
                        roundingMover.ScaleAngleToRadius = true;
                        roundingMover.ManualDecreasingSpeed = 0;
                        float planetDistance = Vector2.Distance(target.transform.Get2DPos(), doneSpawnData.Obj.transform.Get2DPos());
                        roundingMover.SetLinearAndRadiusToSimulateCertainPosition(doneSpawnData.Obj.transform.Get2DPos(), doneSpawnData.Obj.transform.Get2DPos() + mover.Direction * planetDistance / 2);
                        float time = Time.time;
                        DOVirtual.Float(0, 1, float.PositiveInfinity, (v) =>
                        {

                            float timeAfterDelay = Time.time - time;
                            roundingMover.AngleSpeed = angleSpeed.Evaluate(timeAfterDelay);
                        });
                    })
                    .SetLink(doneSpawnData.Obj);
            }).SetLink(doneSpawnData.Obj);
            
            
            return doneSpawnData;

        }
    }
}