using DG.Tweening;
using LetterBattle.Utility;
using UnityEngine;
namespace LetterBattle
{
    public class DieOnLevelEndBehaviour : ISideSpawnBehaviour
    {
        public int PushEffect(in SpawnData data, ComponentsCache cache, SpawnBehavior owner)
        {

            GameObject obj = cache.GameObject;
            DOVirtual.Float(0, 1, float.PositiveInfinity, (_) =>
            {
                if (LevelManager.Current.LevelStatus == LevelManager.Status.WaitingForWin) // ik even would do better
                {
                    UnityEngine.Object.Destroy(obj);
                }
            });
            return 0;
        }
    }
}