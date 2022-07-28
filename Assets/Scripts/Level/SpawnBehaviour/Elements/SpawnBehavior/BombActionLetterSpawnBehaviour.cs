using Cyberultimate;
using Cyberultimate.Unity;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace LetterBattle
{
	public class BombActionLetterSpawnBehaviour : BaseMoverBehaviour
	{
		[SerializeField]
		private BehaviorTable resultTable = null;
		[SerializeField]
		[NaughtyAttributes.MinMaxSlider(2, 4)]
		private Vector2Int howMany = new Vector2Int(1, 2);
		[SerializeField]
		[NaughtyAttributes.MinMaxSlider(12,12)]
		private Vector2 maxPos = new Vector2(1,2 );
		[SerializeField]
		private float angle = 10;
		

		protected override DoneSpawnData InternalSpawn(in SpawnData data)
		{
			var rawLetters = GetRawLetters(data.CustomLetters,data.Side);
			int amount = Randomer.Base.NextInt(howMany[0], howMany[1] + 1);
			char[] pickedLetters = new char[amount];
			for (int i = 0; i < amount; i++)
			{
				pickedLetters[i] = Randomer.Base.NextRandomElement(rawLetters);
			}
			
			DoneSpawnData doneSpawnData = base.InternalSpawn(data);
			BombLetter bomb = doneSpawnData.Obj.GetComponent<BombLetter>();
			bomb.SetSide(data.Side);
			bomb.Init(pickedLetters,resultTable,angle,maxPos,data.Parent,data.Target);
			return doneSpawnData;
		}
	}
}

