using DG.Tweening;
using UnityEngine;

namespace LetterBattle
{
    public class MakeZoneMoveSpawnBehaviour : SpawnBehavior
    {
        //just disclaimer this is kinda hacky
        protected override DoneSpawnData InternalSpawn(in SpawnData data)
        {
            GameObject holder = new GameObject();
            Transform zone = LevelManager.Current.Bases[0].Zone.transform;
            zone.localScale *= 0.8f;
            zone.DOScale(new Vector3(zone.localScale.x * (1/0.8f + 0.3f), zone.localScale.y * (1/0.8f + 0.3f), 1), 1).SetLoops(-1,LoopType.Yoyo)
                .SetLink(holder);
            Transform circle = LevelManager.Current.Bases[0].Circle.transform;
            circle.gameObject.SetActive(false);
            return default;
        }
    }
}