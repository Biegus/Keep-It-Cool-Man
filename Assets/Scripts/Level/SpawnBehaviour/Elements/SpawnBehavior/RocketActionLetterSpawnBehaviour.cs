using Cyberultimate;
using Cyberultimate.Unity;
using DG.Tweening;
using UnityEngine;
using LetterBattle.Utility;
using NaughtyAttributes;

namespace LetterBattle
{

	public class RocketActionLetterSpawnBehaviour : DirectionActionLetterSpawnBehavior
	{
		[SerializeField]
		[NaughtyAttributes.MinMaxSlider(0.1f, 10)]
		private Vector2 rotateTime = new Vector2(1, 2);
		[SerializeField]
		private float speedFactor = 3;
		[SerializeField]
		private bool randomFake=false;// dirty ik, but don't want to break existing isFake
		[SerializeField][AllowNesting][NaughtyAttributes.HideIf(nameof(randomFake))]
		private bool isFake = false;
		[SerializeField]
		private bool disableAnyOtherMoverInFinal = false;
		protected override DoneSpawnData InternalSpawn(in SpawnData data)
		{
			var doneSpawnData = base.InternalSpawn(data);
			var mover = doneSpawnData.Obj.GetComponent<DirectionMover>();
			float delayedTime = Randomer.Base.NextFloat(rotateTime.x, rotateTime.y);
			
			
			
			RocketFlags flags = RocketFlags.None;
			
			flags.ApplyOr(RocketFlags.IsFake, (isFake && !randomFake) || (randomFake && Randomer.Base.NextBool()) );
			
			flags.ApplyOr(RocketFlags.DisableAnyOtherMoveInFinal,disableAnyOtherMoverInFinal);
			
			LettersMoveHelper.MakeARocket(doneSpawnData.Obj, delayedTime, mover, speed * speedFactor, flags,data.Target);
			
			return doneSpawnData;
		}
	}
}