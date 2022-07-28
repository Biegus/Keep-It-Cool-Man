using System;
using Cyberultimate;
using Cyberultimate.Unity;
using LetterBattle.Utility;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Serialization;
namespace LetterBattle
{
	[CreateAssetMenu(menuName = "BehaviourTable")]
	public class BehaviorTable : ScriptableObject, ISerializationCallbackReceiver
	{

		[SerializeField] private LetterType type = null;
		[FormerlySerializedAs("behavior")]
		[SerializeReference]
		private SpawnBehavior behaviour = null;
		[SerializeReference][SerializeReferenceButton]
		private ISideSpawnBehaviour[] sides = new ISideSpawnBehaviour[0];
		public SpawnBehavior Behaviour
		{
			get => behaviour;
			set => behaviour = value;
		}
		
		public GameObject Prefab => type == null ? null : type.Prefab;
		public LetterType Type => type;
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			Refresh();
		}
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.behaviour.SidesRef = sides;
		}
		private void Refresh()
		{
			if (type == null)
			{
				behaviour = null;
				return;
			}
			if (behaviour == null || behaviour.GetType() != type.BehaviourType)
			{
				behaviour = (SpawnBehavior)Activator.CreateInstance(type.BehaviourType);

			}
		}
		public DoneSpawnData Spawn(Vector2 pos, SimpleDirection side, Transform parent, float angleError, Transform target, LettersPackage package = null)
		{
			var dir = Randomer.Base.NextRandomRotation(pos,angleError, target.Get2DPos());
			SpawnData data = new SpawnData(pos, side, parent, Prefab, dir, target,package);

			return Spawn(data);
		}
		public DoneSpawnData Spawn(in SpawnData data)
		{
			var res= this.Behaviour.Spawn(data);

			LEvents.Base.OnSpawnBehaviourUsed.Call(this.Behaviour);
			return res;
		}

#if UNITY_EDITOR
		[CustomEditor(typeof(BehaviorTable))]//forcing normal editor
		public class TableEditor : UnityEditor.Editor
		{

		}
#endif

	}

}