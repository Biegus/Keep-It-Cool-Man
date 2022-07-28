using System;
using System.Collections.Generic;
using System.Linq;
using Cyberultimate;
using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using NaughtyAttributes;
using UnityEngine;
namespace LetterBattle
{
    public class ArmySideSpawnBehaviour : ISideSpawnBehaviour
    {
        public enum PositionModifier
        {
            None,
            FullCurcle,
        }
        [SerializeField] private float delay=0.2f;
        [SerializeField][MinValue(1)] private int times = 1;
        [SerializeField] private PositionModifier posModifier;
        [SerializeField] private bool sameLetters;
        [SerializeField] private bool formWord;

        [NaughtyAttributes.ShowIf(nameof(formWord))][AllowNesting] [SerializeField]
        private List<string> formedWords = new List<string>();

     
        [SerializeField] private bool defend;
        [SerializeField] private bool showLine = false;
            
        
        public int PushEffect(in SpawnData data, ComponentsCache cache, SpawnBehavior owner)
        {
            
            string word = formWord ? Randomer.Base.NextRandomElement(formedWords) : null;

            SpawnData nwData = data;
            if (sameLetters)
            {
                nwData.CustomLetters = new DummyLettersPackage(cache.Get<ActionLetter>().Letter.ToString());
            }

            if (word != null)
                cache.Get<ActionLetter>().SetLetter(word[0]);

            FollowingLine followingLine=null;
            if (showLine)
            {
                followingLine = FollowingLine.Spawn();
                followingLine.PushPoint(cache.GameObject.transform);
                followingLine.DieOnNone = true;
            }
         
            ActionLetter before = defend ? cache.Get<ActionLetter>() : null;

            CorController.Base.Start(CSpawn(nwData, owner,followingLine,before,word,Math.Max(times-1,word?.Length-1??0)),cache.GameObject);
           
            return times;
        }
        private IEnumerator<IWaitable> CSpawn(SpawnData data,SpawnBehavior owner, FollowingLine line, ActionLetter before,string word, int amount)
        {
            SpawnData originalSpawn = data;
            
            for (int i = 0; i < amount; i++)
            {
                yield return Yield.Wait(delay);
                if (owner == null)
                    yield break;
                if (posModifier == PositionModifier.FullCurcle)
                {
                    
                    float angle = (360f / (times ) * (i + 1));
                    data.Pos = (originalSpawn.Pos - data.Target.Get2DPos()).GetRotated(angle) + data.Target.Get2DPos();
                    data.Direction = originalSpawn.Direction.GetRotated(angle);

                }

                if (word != null)
                {
                    data.CustomLetters = new DummyLettersPackage(word[(i+1) % word.Length].ToString());
                }
                var res= owner.Spawn(data,this);
                if (before)// defend mode on
                {
                    var actionLetter= res.Obj.GetComponent<ActionLetter>();
                    Color bf = actionLetter.Renders[0].color;
                    actionLetter.Renders.Foreach(item => item.color = Color.gray);
                    actionLetter.LockGettingDmg.RegisterObj(before);
                    before.OnKilled += (s, e) =>
                    {
                        
                        actionLetter.LockGettingDmg.Unregister(s);
                        actionLetter.Renders.Foreach(item => item.color =bf);
                    };
                    before = actionLetter;
                }
                //don't listen to the warning below, line should always be not null or COMPLETELY NULL. (the guy below doesn't know for shit)
                line?.PushPoint(res.Obj.transform);
            }
        }
    }
}