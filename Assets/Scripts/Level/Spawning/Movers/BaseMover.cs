using System;
using UnityEngine;
using UnityEngine.Serialization;
namespace LetterBattle
{
    public class BaseMover : MonoBehaviour
    {
        protected  float StartTime { get; private set; }
        [FormerlySerializedAs("speedCurve")] [FormerlySerializedAs("speed")] [SerializeField] private ParticleSystem.MinMaxCurve speedCurveCurve = 1;
        public ParticleSystem.MinMaxCurve SpeedCurve
        {
            get => speedCurveCurve;
            set => speedCurveCurve = value;
        }
        public float SpeedCurveValue => speedCurveCurve.Evaluate(Time.time - StartTime);
        public virtual float SpeedScaled => SpeedCurveValue;
        private void Start()
        {
            StartTime = Time.time;
        }   
    }
}