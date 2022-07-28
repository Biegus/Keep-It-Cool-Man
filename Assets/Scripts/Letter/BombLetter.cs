using System.Collections;
using System.Collections.Generic;
using Cyberultimate.Unity;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace LetterBattle
{
    public class BombLetter : Spawnable
    {
        [SerializeField] private GameObject fakeLetterPrefab;

        private char[] pickedLetters=null;
        private BehaviorTable table;
        private DirectionMover mover;
        private float angle;
        private Transform spawnParent;
        private Transform target;
        protected override void Awake()
        {
            base.Awake();
            mover = this.GetComponent<DirectionMover>();
        }
        protected override void OnEnterZone(Collider2D other)
        {
            base.OnEnterZone(other);
            this.Explode();;
        }
        public void Init(char[] pickedLetters, BehaviorTable table, float angle, Vector2 maxPos, Transform spawnParent, Transform target)
        {
            this.pickedLetters = pickedLetters;
            this.table = table;
            this.angle = angle;
            this.spawnParent = spawnParent;
            this.target = target;
           
            foreach (var letter in pickedLetters)
            {
                TMP_Text txt = Object.Instantiate(fakeLetterPrefab, this.transform).GetComponent<TMP_Text>();
                float randomX = Randomer.Base.NextFloat(0, maxPos[0]);
                float randomY = Randomer.Base.NextFloat(0, maxPos[1]);
                txt.transform.localPosition = new Vector2(randomX, randomY);
                txt.text = char.ToUpper(letter).ToString();
                txt.gameObject.transform.rotation = Quaternion.Euler(0,0,Randomer.Base.RandomAngleInDegree()); ;
            }
            
        }
        public void Explode()
        {
            
            if (IsRegisteredForDeath) return;
            for (int i = 0; i < pickedLetters.Length; i++)
            {
                var actionLetter = table.Spawn( this.transform.position, this.Side, spawnParent,0,target).Obj.GetComponent<ActionLetter>();
                
                actionLetter.SetLetter(pickedLetters[i]);
                DirectionMover letterMover = actionLetter.DirectionMover;
                letterMover.Direction = mover.Direction.Rotate(angle - (angle / (pickedLetters.Length - 1)) * 2 * i);
            }
            this.Kill();
            
        }
    }
}

