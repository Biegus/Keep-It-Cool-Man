using Cyberultimate;
using Cyberultimate.Unity;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace LetterBattle
{
    public class VoidReverseSpawnBehaviour : KeyboardRelatedSpawnBehaviour
    {
        [SerializeField]
        private float SpiralRadiusIncrease = 0.2f;

        [SerializeField]
        private ParticleSystem.MinMaxCurve Speed;

        [SerializeField]
        private float SpawnDistanceFromPlayer = 5;
        
        [SerializeField]
        [Header("Duration Variation")]
        private float SpeedCurveDuration = 4;

        [SerializeField] 
        private float DurationVariation = 0.5f;

        [SerializeField] 
        [Header("Movement After Spiral Complete")]
        private bool FiniteSpiral = true;
        
        [SerializeField]
        [NaughtyAttributes.ShowIf(nameof(FiniteSpiral))]
        [NaughtyAttributes.AllowNesting]
        private bool KeepOrbiting = true;

        [SerializeField]
        [NaughtyAttributes.ShowIf(nameof(FiniteSpiral))]
        private bool TargetPlayerAfter = false;
        
        
        
        protected override DoneSpawnData InternalSpawn(in SpawnData data)
        {
            DoneSpawnData doneSpawnData= base.InternalSpawn(data);
            ActionLetter letter = cache.Get<ActionLetter>();
            letter.SetSide(data.Side);
            letter.SetLetter(Randomer.Base.NextRandomElement( GetRawLetters(data.CustomLetters,data.Side)));
            
            RoundingMover mover = cache.Get<RoundingMover>();
            DirectionMover directionMover = cache.Get<DirectionMover>();
            
            directionMover.enabled = false;
            
            mover.ManualDecreasingSpeed = -SpiralRadiusIncrease;
            mover.CurrentRadius = 0;
            
            
            float variation = Randomer.Base.NextFloat(-DurationVariation / 2, DurationVariation / 2);

            //Scale (appearing from the void effect)
            cache.GameObject.transform.localScale = Vector2.zero;
            cache.GameObject.transform.DOScale(1, SpeedCurveDuration + variation)
                .SetLink(cache.GameObject);
            
            //Increase Speed Over Time
            float time = Time.time;
            var target = data.Target;
            DOVirtual.Float(0f, 1f, SpeedCurveDuration + variation, (_) =>
            {
                float timePassed = Time.time - time;

                mover.AngleSpeed = Speed.Evaluate(timePassed);
            })
                .SetLink(cache.GameObject)
                .OnComplete(() => //On Spiral End
                {
                    if (FiniteSpiral)
                    {
                        if (!KeepOrbiting)
                        {
                            directionMover.enabled = true;
                            mover.enabled = false;
                            
                            float angle = mover.Angle * Mathf.Rad2Deg;
                            
                            Vector2 baseVec = Vector2.up;
                            Vector2 direction;
                            if (!TargetPlayerAfter)
                                direction = baseVec.GetRotated(angle);
                            else
                                direction = (Direction)( target.Get2DPos() - doneSpawnData.Obj.transform.Get2DPos()).GetRotated(Randomer.Base.NextFloat( -28,28));// should be from property tho

                            directionMover.Direction = direction.normalized;
                            directionMover.SpeedPlain = mover.AngleSpeed * mover.CurrentRadius; 
                        }
                        else
                            mover.ManualDecreasingSpeed = 0;
                    }
                   
                });

            
            //Change Spawn Location
            Vector2 pos;
            pos = Randomer.Base.NextVector2().normalized;

            pos *= SpawnDistanceFromPlayer;

            mover.Target = data.Target.Get2DPos() + pos;
            
            return doneSpawnData;
        }
    }
}