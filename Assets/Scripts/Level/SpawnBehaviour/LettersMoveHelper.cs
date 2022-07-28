using System;
using Cyberultimate;
using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using UnityEngine;
using UnityEngine.UI;
namespace LetterBattle
{
    [Flags]
    public enum RocketFlags
    {
        None=0,
        DisableAnyOtherMoveInFinal=1<<0,
        IsFake=1<<1,
        DontFlick=1<<2
            
    }
    public class LettersMoveHelper
    {
        public static Tween MakeDisappearLoop(GameObject g, float duration)
        {
            
            var renders = g.GetComponentsInChildren<Graphic>();
            Sequence seq = DOTween.Sequence();
            var withoutAlpha = new Clr(renders[0].color, 0);
            foreach (var render in renders)
            {
                seq.Insert(0,render.DOColor(withoutAlpha, duration / 2).SetLoops(-1, LoopType.Yoyo));
            }
            seq.SetLink(g).SetLoops(-1, LoopType.Yoyo);

            return seq;
        }
       
        public static Tween MakeARocket(GameObject obj,float delayedTime, DirectionMover mover, float speed,RocketFlags flags= RocketFlags.None, Transform target=null)
        {
            if (!flags.Flag(RocketFlags.DontFlick))
                FlickingManager.TryFlick(obj, delayedTime);
            return DOVirtual.DelayedCall(delayedTime, () =>
            {
                if(flags.Flag(RocketFlags.DisableAnyOtherMoveInFinal))
                    obj.GetComponents<BaseMover>().Foreach(mv =>
                    {
                        if (mv != mover)
                            mv.enabled = false;
                    });
                
                mover.enabled = true;// in case default mover would be different
                mover.SpeedPlain = 0;
                mover.Curve = null;
                //var closestPlanet = LevelManager.Current.Bases.MinOriginal(item => (item.transform.Get2DPos() - mover.transform.Get2DPos()).sqrMagnitude);
                var closestPlanet = target;
                float baseAngle = mover.transform.rotation.eulerAngles.z;
                mover.transform.DoRotateAboutZ(360, 1)
                    .OnComplete(() =>
                    {
                        mover.SpeedPlain = speed;
                        Vector2 baseDir = closestPlanet.transform.Get2DPos() - mover.transform.Get2DPos();
                        mover.Direction = !flags.Flag(RocketFlags.IsFake) ? baseDir : baseDir.GetRotated(180);
                    }).SetLink(obj.gameObject);

            }).SetLink(obj.gameObject);
        }
    }
}