#nullable  enable
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;


//this class should be written from the beginning
namespace LetterBattle.Utility
{
    [Serializable]
    public class Cooldown
    {
        
        [SerializeField] private float cooldownTime;
        
        public float CooldownTime => this.cooldownTime;
        public float CurTime => Mathf.Clamp(cooldownTime - (GetTime() - lastTimePos), 0, float.PositiveInfinity);
        public bool Unscaled { get; } = false;
        public event EventHandler<Cooldown> OnFinish = delegate { };
        private float lastTimePos;
        private float GetTime()
        {
            if (Unscaled) return Time.unscaledTime;
            else return Time.time;
        }
        public Cooldown()
        {
            
        }
        public void Finish()
        {
            lastTimePos = GetTime()-(cooldownTime-0.1f);
           
            InternalFinish();
        }
       
        public void Reset()
        {
            lastTimePos =  GetTime();
        }
        private void InternalFinish()
        {
            OnFinish(this, this);
        }
        public Cooldown( float cooldownTime, Action? action=null, bool unscaled=false)
        {
           
            if (cooldownTime < 0) throw new ArgumentOutOfRangeException(nameof(cooldownTime));
            this.cooldownTime = cooldownTime;
            this.Unscaled = unscaled;
            lastTimePos =  GetTime();
        }
        public Cooldown AddEvent(EventHandler<Cooldown> @event)
        {
            OnFinish += @event ?? throw new NullReferenceException(nameof(@event));
            return this;
        }
        public Cooldown AutoUpdate(MonoBehaviour behaviour)
        {
            if (behaviour == null) throw new ArgumentNullException(nameof(behaviour));
            behaviour.StartCoroutine(CUpdating());
            return this;
        }
        private IEnumerator CUpdating()
        {
            while (true)
            {
                this.Update();
                yield return null;
            }
        }
      
        public void Update()
        {
            if (Peek())
                InternalFinish();
         
        }
        public bool Peek()
        {
            return CurTime <= 0;
        }
        public bool Push()
        {
            if (!Peek()) return false;
            lastTimePos = Time.time;
            return true;
        }
        
    }
}