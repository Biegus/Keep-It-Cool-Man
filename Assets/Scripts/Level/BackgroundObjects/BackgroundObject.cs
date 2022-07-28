using Cyberultimate;
using Cyberultimate.Unity;
using System;
using LetterBattle.Utility;
using UnityEditor;
using UnityEngine;

namespace LetterBattle
{
	public class BackgroundObject : MonoBehaviour
	{
		[Serializable]
		public class BackgroundObjectData
		{
			public BackgroundObject prefab;
			public float frequency;
			[NaughtyAttributes.EnumFlags] public FlagDirection spawnSides;
			public Vector2 spawnPosVariation = Vector2.one;
			public float angleVariation = 10f;
		}

		[SerializeField]
		private SimpleDirection intendedSide;

		private float spawnTime;
		private Vector3 camBorder;

		public void Init(BackgroundObjectData objectData)
		{
			//Get random side
			FlagDirection spawnSide = Randomer.Base.NextRandomElement(objectData.spawnSides.ToEnumerable());
			
			//Set pos
			Vector2 spawnDirection = spawnSide.ToDirection().ToVector2();
			Vector2 spawnPosition = spawnDirection * CameraHelper.Current.CameraSize;

			//Randomize spawn pos
			spawnPosition += Randomer.Base.NextVector2(-objectData.spawnPosVariation, objectData.spawnPosVariation);

			this.transform.localPosition = spawnPosition;

			//Rotate depending on chosen side
			SimpleDirection intended = intendedSide;
			float rotFromSide = Vector2.SignedAngle(intended.ToDirection(),spawnSide.ToDirection());

			//Random rotation
			float randomRotation = Randomer.Base.NextFloat(-objectData.angleVariation, objectData.angleVariation);

			//Apply rotation
			float finalRot = rotFromSide + randomRotation;
			this.transform.Rotate(0, 0, finalRot);
			DirectionMover mover = this.GetComponent<DirectionMover>();
			Direction rotated = mover.Direction.Rotate(finalRot);
			mover.Direction = rotated;
		}

		private void Start()
		{
			spawnTime = Time.time;
			//Inflate camera border
			camBorder = (CameraHelper.Current.CameraSize / 2) * 1.2f;
		}

		private void Update()
		{
			if (Time.time - spawnTime > 4)
			{
				float visibleDistance = 3;
				if (LevelManager.Current.LevelStatus != LevelManager.Status.Spawning)
				{
					visibleDistance = 0;
				}
				Vector2 pos = this.transform.localPosition;



				if (Math.Abs(pos.x) > camBorder.x + visibleDistance || Math.Abs(pos.y) > camBorder.y + visibleDistance)
					Destroy(this.gameObject);
			}
		}
	}
}