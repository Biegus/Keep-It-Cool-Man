using LetterBattle.Utility;
using UnityEngine;
namespace LetterBattle
{
    public class LegacySpawnController : ISpawnController
    {
        private int initTime = 0;
        [SerializeField]
        private float spawnCooldown;
        [SerializeField]
        private float time;
    
        public float SpawnCooldown
        {
            get => spawnCooldown;
            set => spawnCooldown = value;
        }
        public float Time
        {
            get => time;
            set => time = value;
        }

        //cooldown of current phase element
        private float curCooldown = 0;
        //global cooldown the whole phase will stop after it
        private Cooldown timeCooldown;
        public float GetTimeInfo()
        {
            return time;
        }
        public void Init(Phase phase)
        {
            timeCooldown  = new Cooldown( time);
            initTime++;
        }
        public bool Update(Phase phase)
        {
            if (timeCooldown == null) return false;
            if (timeCooldown.Peek()) return false;
          while (curCooldown <= 0)
          {
              var element = phase.GetRandomElementUsingPriority();
              DoneSpawnData data= phase.Spawn(element);
              if (data.AbsoluteCount == 0)
              {
                  data.AbsoluteCount = 1;
              }
              if (element!=null &&element.CustomCooldown.HasValue())
                  curCooldown += element.CustomCooldown.GetValue() * data.AbsoluteCount;
              else
                curCooldown += spawnCooldown * data.AbsoluteCount;
          }
          curCooldown -= UnityEngine.Time.deltaTime;
          return true;
        }
        public void Dispose(Phase phase)
        {
            
        }
        public string GetDescription(Phase phase)
        {
            return "Standart way of handling this. After fixed amount of time random element is picked and spawned. Some object can caused next to wait a little " +
                   "longer if custom cooldown is set";
        }

    }
}