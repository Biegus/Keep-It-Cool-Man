using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
namespace LetterBattle
{
    public class DisappearSideSpawnBehaviour: ISideSpawnBehaviour
    {

        [SerializeField][Range(0.2f,4f)] private float duration;
        public int PushEffect(in SpawnData data, ComponentsCache cache, SpawnBehavior owner)
        {
            LettersMoveHelper.MakeDisappearLoop(cache.GameObject, duration/2);
            return 0;
        }
    }
}