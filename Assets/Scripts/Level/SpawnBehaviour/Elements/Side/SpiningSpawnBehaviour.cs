using LetterBattle.Utility;
using NaughtyAttributes;
using UnityEngine;
namespace LetterBattle
{
    public class SpiningSpawnBehaviour : ISideSpawnBehaviour
    {
        [AllowNesting]
        
        [Range(0.1f,5)]
        [SerializeField] private float frequency=2;
      
        public int PushEffect(in SpawnData data, ComponentsCache cache, SpawnBehavior owner)
        {
            SpaceSpinner spinner = cache.Get<SpaceSpinner>();
            if (spinner == null)
                spinner = cache.GameObject.AddComponent<SpaceSpinner>();
           
            spinner.Frequency *= frequency;
            return 0;
        }
    }
}