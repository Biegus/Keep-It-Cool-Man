using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace LetterBattle
{
    public class ClusterSpawnBehaviour : KeyboardRelatedSpawnBehaviour
    {
        [SerializeField]
        private Vector2 ClusterSize;
        
        [SerializeField]
        private int LetterQuantity = 10;
        
        [SerializeField]
        private int Integrity = 2;

        [Header("Movement")]
        [SerializeField]
        private float Speed = 1;
        
        [SerializeField] 
        private bool Rotate = false;

        [SerializeField] 
        [NaughtyAttributes.ShowIf(nameof(Rotate))]
        [NaughtyAttributes.AllowNesting]
        private float FullRotationDuration = 20;

        //TODO: Clusters are so big they spawn on screen and we have to fix that i guess
        protected override DoneSpawnData InternalSpawn(in SpawnData data)
        {
            SpawnData Data = data;
            
            char letter = (Randomer.Base.NextRandomElement( GetRawLetters(data.CustomLetters,data.Side))); 
            
            //Move cluster out of the screen
            Data.Pos -= (data.Target.transform.Get2DPos() - data.Pos).normalized * ClusterSize;

            return new DoneSpawnData(ClusterSpawner.SpawnCluster(Data, letter, LetterQuantity, ClusterSize, Integrity, this, Rotate, FullRotationDuration, Data.Pos, Speed),LetterQuantity);
        }
        
    }
    
}