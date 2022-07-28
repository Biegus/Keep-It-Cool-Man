using System;
using LetterBattle.Utility;
using UnityEngine;

namespace LetterBattle
{
    public class LetterCollideSideBehaviour : ISideSpawnBehaviour
    {
        //private ActionLetter SelfLetter;
        
        public int PushEffect(in SpawnData data, ComponentsCache cache, SpawnBehavior owner)
        {
            ActionLetter SelfLetter = cache?.GameObject.GetComponent<ActionLetter>();

            if (SelfLetter != null)
                SelfLetter.OnTriggerEnter += (s,e)=>Collided(s,e,SelfLetter);
            return 0;
        }

        private void Collided(object sender, Collider2D other,ActionLetter SelfLetter)
        {
            ActionLetter letter = other.GetComponent<ActionLetter>();
            if (letter != null &&
                letter.Letter != SelfLetter.Letter)
            {
                
                letter.Kill();
                SelfLetter.Kill();
            }
        }
        
    }
}