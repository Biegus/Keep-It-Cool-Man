using UnityEngine;
namespace LetterBattle
{
	public class VinylHand : MonoBehaviour
	{
		[SerializeField] private AnimationCurve curve;
		[SerializeField] private float factor = 1;
		private float baseRotation;

		[SerializeField]
		private Transform firstHand = null;

		private void Awake()
		{
			baseRotation = firstHand.eulerAngles.z;
		}
		private void Update()
		{
			firstHand.localRotation = Quaternion.Euler(0, 0, baseRotation + curve.Evaluate(Time.time) * factor);
		}
	}
}