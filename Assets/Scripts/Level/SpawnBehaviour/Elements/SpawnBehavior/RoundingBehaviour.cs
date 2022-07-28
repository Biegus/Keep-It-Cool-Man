using System;
using Cyberultimate;
using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using UnityEngine;
namespace LetterBattle
{
    public class RoundingBehaviour : KeyboardRelatedSpawnBehaviour
    {
        public enum Mode
        {
            Normal=0,
            Sin,
        }
        [SerializeField]
        private float angleSpeed;
        [SerializeField]
        private float radiusSpeed;

        [SerializeField] private Mode mode;
        protected override DoneSpawnData InternalSpawn(in SpawnData data)
        {
            DoneSpawnData doneSpawnData= base.InternalSpawn(data);
            ActionLetter letter = doneSpawnData.Obj.GetComponent<ActionLetter>();
            letter.SetSide(data.Side);
            letter.SetLetter(Randomer.Base.NextRandomElement( GetRawLetters(data.CustomLetters,data.Side)));
            RoundingMover mover = doneSpawnData.Obj.GetComponent<RoundingMover>();
            mover.SetLinearAndRadiusToSimulateCertainPosition(doneSpawnData.Obj.transform.position,data.Target.Get2DPos());
            mover.ManualDecreasingSpeed = radiusSpeed;
            mover.AngleSpeed = angleSpeed;
            switch (mode)
            {
                case Mode.Sin:
                    mover.RadiusModif = (float theta) => Mathf.Sin(5 * theta) / 5;   
                    break;
                default: break;
            }
            mover.gameObject.DoUpdate(() =>
            {
                if (mover.CurrentRadius == 0)
                    doneSpawnData.Obj.transform.DOScale(Vector2.zero, 1f).SetLink(doneSpawnData.Obj)
                        .OnComplete(() =>
                        {
                            letter.SpawnParticleFlag = false;
                            letter.Kill();
                        });
            });
            
            return doneSpawnData;
        }
    }
}