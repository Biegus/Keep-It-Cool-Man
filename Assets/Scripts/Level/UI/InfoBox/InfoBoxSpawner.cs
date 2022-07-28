using Cyberultimate.Unity;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
namespace LetterBattle
{
	public class InfoBoxSpawner : MonoSingleton<InfoBoxSpawner>
	{
		[SerializeField] private Transform parent;
		[FormerlySerializedAs("text")] [SerializeField] [PrefabObjectOnly] private InfoBox prefab;
		[SerializeField] private Transform[] placesToSpawn = new Transform[0];
		private int stronglyAlive = 0;
		private int index = 0;
		public InfoBox Spawn(Vector2? position, string text, Color color, float time = 0.8f)
		{

			if (stronglyAlive > 5) return null;
			InfoBox infoBox = Instantiate(prefab, parent, false);
			infoBox.transform.position = position ?? placesToSpawn[index = (index + 1) % placesToSpawn.Length].transform.position;

			Tween tween = DOVirtual.DelayedCall(time, () => stronglyAlive--,true);

			infoBox.Init(text, color, time);
			stronglyAlive++;

			return infoBox;
		}

	

	}
}