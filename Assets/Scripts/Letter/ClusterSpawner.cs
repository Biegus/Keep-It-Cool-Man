using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using UnityEngine;

namespace LetterBattle
{
    public class ClusterSpawner
    {
        public static GameObject SpawnCluster(in SpawnData data,char letterChar,
            int LetterQuantity,Vector2 ClusterSize,int Integrity,SpawnBehavior spawnBehaviour, bool Rotate, float FullRotationDuration, Vector2 position, float Speed)
        {
            GameObject obj = UnityEngine.Object.Instantiate(data.Prefab,data.Parent,false);
            obj.transform.SetAsFirstSibling();
            
            var letter = obj.GetComponent<ClusterLetter>();

            letter.Letter = letterChar;
            letter.SetSide(data.Side);

            letter.transform.position = position;
            
            letter.Init(
                ClusterSize,
                data.Target,
                data.Direction,
                LetterQuantity,
                Integrity,
                spawnBehaviour,
                data,
                Rotate,
                FullRotationDuration,
                Speed
            );

            if (Rotate)
            {
                obj.transform.DoRotateAboutZ(360, FullRotationDuration)
                    .SetLoops(-1)
                    .SetEase(Ease.Linear)
                    .SetLink(obj);
            }

            return obj;
        }
    }
}