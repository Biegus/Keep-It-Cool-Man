using System;
using DG.Tweening;
using UnityEngine;
namespace LetterBattle.Utility.Utility
{
    public class Disapearer: TweenSpawnerMono
    {
        protected override Tween ConstructTween()
        {
            if(tween==null)// ignore if value has changed
                return LettersMoveHelper.MakeDisappearLoop(this.gameObject, Duration);
            return null;

        }


    }
}