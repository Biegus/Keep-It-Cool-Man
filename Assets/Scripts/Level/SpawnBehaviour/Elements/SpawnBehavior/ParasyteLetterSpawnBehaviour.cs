using Cyberultimate.Unity;
using LetterBattle.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LetterBattle
{
	public class ParasyteLetterSpawnBehaviour : ActionLetterSpawnBehaviour
	{
		protected override DoneSpawnData InternalSpawn(in SpawnData data)
		{

			
			DoneSpawnData spawnData = base.InternalSpawn(data);
			RoundingMover roundMover = spawnData.Obj.AddComponent<RoundingMover>();

			roundMover.CurrentRadius = 2.137f;
			roundMover.AngleSpeed = 1.5f;
			
			DirectionMover dirMover = spawnData.Obj.GetComponent<DirectionMover>();
			dirMover.Direction = data.Direction;

			StartPhaseOne();

			var target = LevelManager.Current.Targets[1];
			bool collided = false;
			bool deadLetter = false;



			void StartPhaseOne()
			{
				roundMover.enabled = false;
				dirMover.enabled = true;
			}
			Cooldown c = null;
			ActionLetter actionLetter = cache.Get<ActionLetter>();
			actionLetter.MaxVisibleDistance = 9000;

			DoTweenHelper.DoUpdate(spawnData.Obj, () => 
			{
				if (!deadLetter)
				{
					if (!collided && (target.Get2DPos() - spawnData.Obj.transform.Get2DPos()).sqrMagnitude < roundMover.CurrentRadius * roundMover.CurrentRadius)
					{
						dirMover.enabled = false;
						roundMover.enabled = true;
						Vector2 toTheTarget = target.Get2DPos() - spawnData.Obj.transform.Get2DPos();
						roundMover.Angle = Mathf.Atan2(-toTheTarget.y, -toTheTarget.x);
						c = new Cooldown(3);
						collided = true;
						actionLetter.LockGettingDmg.RegisterObj(this);
					}

					if (collided)
					{


						if (c.Push())
						{
							deadLetter = true;
							actionLetter.LockGettingDmg.Unregister(this);
							StartPhaseOne();
							return;
						}

						roundMover.Target = target.position;
					}
				}


			});



			return spawnData;


		}


	}
}
