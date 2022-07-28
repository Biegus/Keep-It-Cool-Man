using Cyberultimate;
using Cyberultimate.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace LetterBattle
{
	public class KeyboardRelatedSpawnBehaviour : SpawnBehavior
	{
		[SerializeField][FormerlySerializedAs("letters")] private LettersPackage defaultLetters = null;



	
		public  ILettersPackage GetPackage(in SpawnData data) { return data.CustomLetters!=null ? (ILettersPackage) data.CustomLetters : (ILettersPackage)defaultLetters;}
		protected override DoneSpawnData InternalSpawn(in SpawnData data)
		{
			return base.InternalSpawn(data);
			
		}
		
		protected char[] GetRawLetters(ILettersPackage package ,SimpleDirection side)
		{
			// get up, left, right, down letters... can be multiply
			return (package?? defaultLetters).GetLetters(side).ToArray();
		}
		public override ILettersPackage GetDefLetters()
		{
			return defaultLetters;
		}
		
	}
}


