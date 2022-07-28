using Cyberultimate;
using Cyberultimate.Unity;
using System.Linq;
using UnityEngine;
namespace LetterBattle
{
	public class 
		DirectionActionLetterSpawnBehavior : BaseMoverBehaviour
	{
		[SerializeField] private float animFactor = 1;
		protected override DoneSpawnData InternalSpawn(in SpawnData data)
		{
			if (GetPackage(data).GetLetters(data.Side)== string.Empty)
			{
				Debug.LogWarning($"There is no letter for {data.Side}");
			}

			DoneSpawnData obj = base.InternalSpawn(data);
			
			ActionLetter actionLetter = cache.Get<ActionLetter>();
			actionLetter.SetSide((SimpleDirection)data.Side);
		
			actionLetter.DirectionMover.AnimationModif *= animFactor;
			return obj;
		}
	}
}