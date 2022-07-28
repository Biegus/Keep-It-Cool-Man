using DG.Tweening;
using UnityEngine;
using LetterBattle.Utility;
using NaughtyAttributes;

namespace LetterBattle
{
    public class SizeFlexibleSideSpawnBehaviour : ISideSpawnBehaviour
    {
        [SerializeField] 
        [CurveRange(0,0,float.PositiveInfinity,float.PositiveInfinity,EColor.Green)]
        private AnimationCurve animCurve;
        
        public int PushEffect(in SpawnData data, ComponentsCache cache, SpawnBehavior owner)
        {
            cache.GameObject.transform.localScale *= 0.5f;
            
            Vector2 scale = cache.GameObject.transform.localScale;

            float time = Time.time;

            DOVirtual.Float(0f, 1f, float.PositiveInfinity, (_) =>
            {
                float timePassed = Time.time - time;

                cache.GameObject.transform.localScale = scale * animCurve.Evaluate(timePassed);

            }).SetLink(cache.GameObject);

            /*cache.GameObject.transform.DOScale(scale * 2.5f, scaleTime)
                .SetLoops(-1,LoopType.Yoyo)
                .SetEase(animCurve)
                .SetLink(cache.GameObject);*/
            return 0;
        }
    }
}