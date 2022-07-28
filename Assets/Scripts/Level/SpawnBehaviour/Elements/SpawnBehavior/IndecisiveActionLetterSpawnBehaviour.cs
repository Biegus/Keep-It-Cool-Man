using System;
using System.Linq;
using Cyberultimate;
using Cyberultimate.Unity;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
namespace LetterBattle
{
    public class IndecisiveActionLetterSpawnBehaviour: DirectionActionLetterSpawnBehavior
    {
        [Serializable]
        public struct SpeedPhase
        {
            
            [NaughtyAttributes.MinMaxSlider(.1f,5)]
            [AllowNesting]
            public Vector2 time;
            public bool DecideOneForInstance;
            [Range(0.1f,3f)]
            public float speedModifier;

        }
        [SerializeField] private bool dontLoop = false;
        [SerializeField] private SpeedPhase[] phases = new SpeedPhase[0];
        protected override DoneSpawnData InternalSpawn(in SpawnData data)
        {
           
            var doneSpawnData = base.InternalSpawn(data);
            DirectionMover action = doneSpawnData.Obj.GetComponent<DirectionMover>();

            float[] selectedTime = phases.Select(item => Randomer.Base.NextFloat(item.time.x, item.time.y)).ToArray();
            float got = 0;
            int index = 0;
            float sum = selectedTime.Sum();

            var tween = DOVirtual.Float(0, sum, sum, (v) =>
            {
                action.SpeedPlain = phases[index].speedModifier* speed;
                if (index == phases.Length-1) { return; }
                ;
                if (v - got >= selectedTime[index])
                {
                    got += selectedTime[index];
                    index++;

                }
            }).OnStepComplete((() =>
                    {
                        selectedTime = phases.Select((item, i) =>
                            {
                                if (item.DecideOneForInstance) return selectedTime[i];
                                else return Randomer.Base.NextFloat(item.time.x, item.time.y);
                            }
                        ).ToArray();
                        index = 0;
                        got = 0;
                    }
                ));
            if (!dontLoop) tween.SetLoops(-1);
            return doneSpawnData;
        }
    }
}