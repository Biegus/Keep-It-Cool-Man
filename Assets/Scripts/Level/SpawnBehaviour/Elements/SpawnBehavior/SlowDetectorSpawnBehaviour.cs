using System;
using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using NaughtyAttributes;
using UnityEngine;
namespace LetterBattle
{
    public class SlowDetectorSpawnBehaviour : DirectionActionLetterSpawnBehavior
    {
        [Serializable]
        struct Change
        {
            public float Delay;
            public float Angle;
        }
        [SerializeField] private bool forceStartAngle = false;
        [SerializeField][AllowNesting][NaughtyAttributes.ShowIf(nameof(forceStartAngle))] private float startAngle = 0;
        [SerializeField][NaughtyAttributes.CurveRange(0,0,1,1,EColor.Orange)] private AnimationCurve speedOverChangeCurve;
        [SerializeField] private Change[] delays = new Change[0];
        
     
        protected override DoneSpawnData InternalSpawn(in SpawnData data)
        {
           
            #if DEBUG
            if (delays.Length ==0)
            {
                throw new ArgumentException("There is no delay set to SlowDetector");
            }
            #endif
            DoneSpawnData doneSpawnData = base.InternalSpawn(data);
            DirectionMover mover = cache.Get<DirectionMover>();
            Transform target = data.Target;
            void SetAngle( float angle)
            {
                Vector2 baseDir = (target.Get2DPos() - doneSpawnData.Obj.transform.Get2DPos());
                mover.Direction = baseDir.GetRotated(angle * Randomer.Base.NextPositiveOrNegative());
            }
            if(forceStartAngle)
                SetAngle(startAngle);
            int index = -1;
            float startSpeed = mover.SpeedPlain;
            void MoveDelay()
            {
                if (index == delays.Length - 1)
                    return;
                float startSpeed = mover.SpeedPlain;
                DOTween.To(() => mover.SpeedPlain, (v) => mover.SpeedPlain = v, mover.SpeedPlain , delays[++index].Delay)
                    .SetLink(doneSpawnData.Obj.gameObject)
                    .SetEase(speedOverChangeCurve)
                    .OnComplete(() =>
                    {
                        SetAngle(delays[index].Angle);
                        MoveDelay();
                        mover.SpeedPlain = startSpeed;
                    });




            }
            MoveDelay();

            return doneSpawnData;
        }
    }
}