using System;
using Cyberultimate.Unity;
using UnityEngine;
using UnityEngine.Serialization;
namespace LetterBattle
{
    public class RoundingMover : BaseMover
    {
        
     
        [SerializeField]
        private float defaultRadius;
        [SerializeField]
        private float defaultAngle;
        [SerializeField]
        private bool useLocal = false;
        
        [SerializeField]
        private float defaultDecreasingSpeed;
        [FormerlySerializedAs("defaultAlphaSpeed")] [SerializeField] private float defaultAngleSpeed;
        
        public float ManualDecreasingSpeed { get; set; }
        public float AngleSpeed { get; set; }
        public float Angle { get; set; }
        public Vector2 Target { get; set; } = Vector2.zero;
        public bool ScaleAngleToRadius { get; set; } = false;
        public Func<float, float> RadiusModif { get; set; } = null;
        public float CurrentRadius
        {
            get => _CurrentRadius;
            set
            {
                if (value == _CurrentRadius) return;
                else if (value < 0) throw new ArgumentException("value has to be greater or equal to zero");
                _CurrentRadius = value;
            }
        }
        private float _CurrentRadius;
        private Vector2 pos;
        private float radiusBefore=-1;
        private float distBefore = 0;
        private ActionLetter actionLetter;
        private void Awake()
        {
            ManualDecreasingSpeed = defaultDecreasingSpeed;
            CurrentRadius = defaultRadius;
            AngleSpeed = defaultAngleSpeed;
            Angle = defaultAngle;
        }
        private void OnEnable()
        {
            actionLetter = this.GetComponent<ActionLetter>();
            if (actionLetter)
            {
                distBefore = actionLetter.MaxVisibleDistance;

                actionLetter.MaxVisibleDistance = float.MaxValue;
            }
        }
        private void OnDisable()
        {
            if (actionLetter) actionLetter.MaxVisibleDistance = distBefore;
        }
        public void SetLinearAndRadiusToSimulateCertainPosition(Vector2 pos, Vector2 target)
        {
            Vector2 toTheTarget= target - pos;
            CurrentRadius = toTheTarget.magnitude;
            Angle = Mathf.Atan2(-toTheTarget.y, -toTheTarget.x);

            Target = target;
        }
        private void Update()
        {
            RefreshRadiusAndLinear();
            UpdateLocation();
        }
        
        private void RefreshRadiusAndLinear()
        {
            if (CurrentRadius != 0)
            {
                float angleTemp = AngleSpeed * Time.deltaTime * SpeedCurveValue;
                if (ScaleAngleToRadius) angleTemp /= CurrentRadius;
                Angle += angleTemp;
                radiusBefore = CurrentRadius;
            }
          
 
            CurrentRadius = Mathf.Max(CurrentRadius- ManualDecreasingSpeed * Time.deltaTime*SpeedCurveValue,0) ;
        }
        public void UpdateLocation()
        {

            Func<float, float> radiusModf= RadiusModif?? ((v)=>0);

            float x =Target.x+ Mathf.Cos(Angle)* (CurrentRadius+radiusModf(Angle)*CurrentRadius);
            float y = Target.y+ Mathf.Sin(Angle)*(CurrentRadius+radiusModf(Angle)*CurrentRadius);
            if (!useLocal)
                this.transform.position = new Vector3(x, y);
            else
                this.transform.localPosition = new Vector3(x, y);
        }
    }
}