using DG.Tweening;
using UnityEngine;
namespace LetterBattle.Utility.Utility
{
    public abstract class TweenSpawnerMono : MonoBehaviour
    {
        [SerializeField]
        private float duration = 7f;
        
        public float Duration
        {
            get => duration;
            set
            {
                if (value == duration) return;
                duration = value;
               
                BuildTween();

            }
        }
        public float Frequency
        {
            get => 1 / Duration;
            set =>Duration= 1 / value ;
        }
        protected Tween tween;
       
        protected  virtual void Awake()
        {
            BuildTween();
        }
        protected void BuildTween()
        {
         
            var  temp = ConstructTween();
            if (temp != null)
            {
                tween.Kill();
                tween = temp;
                if (!this.enabled)
                    tween.Pause();
            }
           

        }
        protected abstract  Tween ConstructTween();
        
        protected  virtual void OnDisable()
        {
            tween?.Pause();
        }
        protected  virtual void  OnEnable()
        {
            tween?.Play();
        }
    }
}