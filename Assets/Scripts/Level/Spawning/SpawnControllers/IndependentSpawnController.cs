using System.Collections.Generic;
using System.Linq;
using LetterBattle.Utility;
using NaughtyAttributes;
using UnityEngine;
namespace LetterBattle
{
    public class IndependentSpawnController : ISpawnController
    {
        [SerializeField]
        private float time;
        [SerializeField]
        private float defCooldown = 1;
        [SerializeField] // if it's checked first spawn will appear perfectly at the start of phase
        private bool firstExactlyAtStart = true;

        [SerializeField] private bool useAnimationCurve;
        [NaughtyAttributes.CurveRange(0.0f,0.1f,1,5,EColor.Red)]
        [NaughtyAttributes.ShowIf("useAnimationCurve")]
        [SerializeField] private AnimationCurve curve;
        private float[] curTimes;
        private float[] spawnCooldowns;
        private Cooldown timeCooldown;
        private float startTime;


        public float GetTimeInfo()
        {
          
            return time;
        }
        public void Init(Phase phase)
        {
            startTime = Time.time;
            timeCooldown  = new Cooldown( time);
            spawnCooldowns =
                (from item in phase.Elements
                    select (item.CustomCooldown.HasValue() ? item.CustomCooldown.GetValue() : defCooldown))
                .ToArray();
            if (firstExactlyAtStart)
                curTimes = new float[spawnCooldowns.Length];
            else
                curTimes = Enumerable.Range(0, spawnCooldowns.Length).Select((i) => spawnCooldowns[i]).ToArray();

        }

        private float GetAnimFactor()
        {
            if (!useAnimationCurve) return 1;
            else return  curve.Evaluate( ((Time.time - startTime) / time));
        }
        public bool Update(Phase phase)
        {
            if (timeCooldown.Peek()) return false;
            for (int i = 0; i < phase.Elements.Count; i++)
            {
                while (curTimes[i] <= 0)
                {
                    DoneSpawnData data= phase.Spawn(phase.Elements[i]);
                    if (data.AbsoluteCount == 0) // something went wrong, probably there was nothing to spawn, let's just pretend something spawned
                    {
                        data.AbsoluteCount = 1;
                    }
                    curTimes[i] += spawnCooldowns[i] * GetAnimFactor() * data.AbsoluteCount;
                }
                curTimes[i] -= UnityEngine.Time.deltaTime;
            }
            return true;
        }
        public void Dispose(Phase phase)
        {
            curTimes = null;
        }
        public string GetDescription(Phase phase)
        {
            return $"Descr:Each element has its own timer that is not dependant on other elements one. Priority will be ignored\n" +
                   $"Data: {1f/(defCooldown/ phase.Elements.Count)} elements per second (assuming defCooldown)";
        }

    }
}