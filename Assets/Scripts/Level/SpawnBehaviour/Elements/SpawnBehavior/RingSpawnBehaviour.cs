using Cyberultimate.Unity;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
namespace LetterBattle
{
    public class RingSpawnBehaviour : DirectionActionLetterSpawnBehavior
    {
        public enum SpawnType
        {
            Circle,
            Inverse,
        }
       
        [SerializeField][AllowNesting][NaughtyAttributes.Expandable][ NaughtyAttributes.Required()] private BehaviorTable insider;
        [SerializeField] private float delay=1;
        [SerializeField] private int amount = 4;
        [SerializeField] private bool explodeWhenKilled = true;
        [SerializeField] private bool flick = true;
        [SerializeField] private SpawnType spawnType = SpawnType.Circle;
        [SerializeField] private int changeTarget=-1;
        [SerializeField] private bool hideLines;
        protected override DoneSpawnData InternalSpawn(in SpawnData data)
        {
            DoneSpawnData doneSpawnData = base.InternalSpawn(in data);
            GameObject obj = doneSpawnData.Obj;
            ActionLetter actionLetter = cache.Get<ActionLetter>();
          
            Direction org = data.Direction;
            Transform target = data.Target;
            if (flick)
            {
                FlickingManager.TryFlick(obj, delay);
            }
            SpawnData template = data;
            void Explode()
            {
                
                if(changeTarget!=-1)
                {
                    target = LevelManager.Current.Targets[changeTarget];
                    Debug.Log(target);
                }
                template.Target = target;
                
                
                actionLetter.Kill(ActionLetter.DeathType.Unknown);
                float mv = Randomer.Base.NextFloat(0, 90);
                FollowingLine line;
                if (!hideLines)
                {
                     line = FollowingLine.Spawn();
                    line.DieOnNone = true;
                }
                else line = null;
                
                
                int k = amount;
                for (int i = 0; i < amount; i++)
                {
                    if (spawnType == SpawnType.Circle)
                    {
                        template.Direction = Direction.Right.Rotate(i / (float)k * 360 + mv);
                        
                    }
                    else
                    {
                        template.Direction = (target.Get2DPos() - obj.transform.Get2DPos()).normalized.GetRotated(180-45 + i / ((float)k-1) * 90 + mv);
                    }


                    template.Pos = obj.transform.Get2DPos() + template.Direction * 0.2f;
                 
                    var nwSpawned= insider.Spawn(template);
                    line?.PushPoint(nwSpawned.Obj.transform);

                }
            }
            DOVirtual.DelayedCall(delay, () =>
            {
                Explode();

            }).SetLink(obj);
            if (explodeWhenKilled)
                actionLetter.OnKilled += (s, e) =>
                {
                    if ((s as ActionLetter).DeathReason == ActionLetter.DeathType.KilledByPlayer)
                        Explode();
                };
            doneSpawnData.AbsoluteCount = amount + 1;
            return doneSpawnData;

        }
    }
}