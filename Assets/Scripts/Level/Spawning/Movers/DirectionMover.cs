using System;
using Cyberultimate.Unity;
using UnityEngine;
using UnityEngine.Serialization;
namespace LetterBattle
{
    public class DirectionMover : BaseMover
    {
        [SerializeField] private Direction direction=Direction.Down;
        [SerializeField] private AnimationCurve curve=null;
        public AnimationCurve Curve
        {
            get => curve;
            set => curve = value;
        }
    
        public Direction Direction
        {
            get => direction;
            set => direction = value;
        }
     
        public float SpeedPlain { get; set; } = 1;
        public float AnimationModif { get; set; } = 1;
        private Vector2 AnimVector => new Vector2(-direction.Y, direction.X);
        
        private float animPos = 0;
        
      
       
        protected virtual void Update()
        {
            float newAnimPos = animPos + SpeedScaled;
            Vector2 animVector2 = Vector2.zero;
            if (curve != null)
            {
               animVector2= AnimVector * ((curve.Evaluate(newAnimPos) - curve.Evaluate(animPos)) * AnimationModif);
            }
          
            this.transform.position = this.transform.Get2DPos() + direction *  SpeedScaled + animVector2;
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
            animPos = newAnimPos;
        }
        public override float SpeedScaled=>Time.deltaTime* Speed;
        public float Speed => SpeedCurveValue * SpeedPlain;

    }
}