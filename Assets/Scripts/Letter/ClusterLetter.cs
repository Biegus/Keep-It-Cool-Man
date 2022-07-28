using System;
using System.Collections.Generic;
using System.Text;
using Cyberultimate.Unity;
using LetterBattle.Utility;
using UnityEngine;

namespace LetterBattle
{
    public class ClusterLetter : Spawnable
    {
        [SerializeField] 
        private BehaviorTable BehaviourTable;
        
        public char Letter { get; set; }
        public int LettersDead { get; private set; } = 0;
        
        private SpawnBehavior spawnBehaviour;
        private SpawnData spawnData;
        private int integrity = 1;
        private int letterCount = 0;
        private int splitInto = 3;
        private Vector2 chunkSize;
        private bool rotate;
        private float rotateDuration;

        public virtual void Init(Vector2 PositionVariation,Transform target,Direction direction,
            int LetterCount,int integrity,SpawnBehavior spawnBehaviour,SpawnData data,bool Rotate,float FullRotationDuration,float Speed)
        {
            this.spawnBehaviour = spawnBehaviour;
            this.spawnData = data;
            this.integrity = integrity;
            this.letterCount = LetterCount;
            this.chunkSize = PositionVariation;
            this.rotate = Rotate;
            this.rotateDuration = FullRotationDuration;
            
            DirectionMover.Direction = direction;
            DirectionMover.SpeedPlain = Speed;
            
            for (int i = 0; i < LetterCount; i++)
            {
                Vector2 spawnPos = Randomer.Base.NextVector2(-PositionVariation / 2, PositionVariation / 2);

                spawnPos += this.transform.Get2DPos();
                
                ActionLetter letter = BehaviourTable.Spawn(spawnPos, this.Side, this.transform, 0, target)
                    .Obj.GetComponent<ActionLetter>();

                letter.DirectionMover.enabled = false;
                letter.SetLetter(this.Letter);
                
                foreach (var sideEffect in spawnBehaviour.SidesRef)
                {
                    sideEffect.PushEffect(data,new ComponentsCache(letter.gameObject),spawnBehaviour);
                }
               
                
                letter.OnKilled += new EventHandler(OnLetterKilled);
                
            }

            
        }

        private void OnLetterKilled(object sender,EventArgs args)
        {
            LettersDead++;

            if (LettersDead >= integrity)
            {
                SplitChunk();
            }
        }
        
        public void SplitChunk()
        {
            if (LevelManager.Current.LevelStatus != LevelManager.Status.Spawning) return;
            if (letterCount < 5)
                return;

            for (int i = 0; i < splitInto; i++)
            {
                int NextLetterCount = (letterCount - LettersDead) / splitInto;
                int NextIntegrity = Mathf.Clamp(integrity / splitInto,1,NextLetterCount);
                float NextSpeed = DirectionMover.SpeedPlain * 1.1f;

                Vector2 previousDirection = DirectionMover.Direction;

                float k = 45f;
                float additionalAngle = ((2f * k) / (splitInto - 1)) * i - k;
                
                spawnData.Direction = previousDirection.GetRotated(additionalAngle).normalized;
                
                ClusterSpawner.SpawnCluster(spawnData, Letter, NextLetterCount, chunkSize / splitInto, NextIntegrity, spawnBehaviour,
                    rotate, rotateDuration, this.transform.Get2DPos(),NextSpeed);
            }

            this.Kill();
        }
    }
}