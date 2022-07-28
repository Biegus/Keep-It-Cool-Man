using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace LetterBattle.Utility
{
	public class DoTweenStraight : MonoBehaviour
	{
		[SerializeField] private Transform center;

		[SerializeField]
		private Transform offset = null;
		[SerializeField] private float firstPhaseDuration = 1;
		[SerializeField] private float diff = 1;
		[SerializeField] private float secondPhaseDuration = 1;
		[SerializeField] private float minDistance = 100;
		[SerializeField] private float maxDistance = 1000;
		[SerializeField] private float angleVariations = 10;

		[SerializeField]
		private float dividePower = 16f;

		private void Start()
		{

			RectTransform thisRect = this.GetComponent<RectTransform>();

			var children = this.transform.GetChildren().Select(item => item.gameObject).ToArray();
			for (int i = 0; i < children.Length; i++)
			{
				GameObject obj = children[i];
				Debug.Log(offset.localPosition.x);

				RectTransform rect = obj.GetComponent<RectTransform>();

				Vector2 randomDir = (center.transform.position - obj.transform.position).normalized;
				randomDir = randomDir.GetRotated(Randomer.Base.NextFloat(-angleVariations, +angleVariations));
				// var (pos, angle) = CircleSettingHelper.GetPosAndAngle(data, (float)i / (children.Length - 1));
				Sequence seq = DOTween.Sequence();



				// dir for random drop letter
				var vec = randomDir * Randomer.Base.NextFloat(minDistance, maxDistance);
				float trueDuration = firstPhaseDuration + diff * i;
				seq.Append(rect.DOAnchorPos(rect.anchoredPosition - thisRect.anchoredPosition + vec, trueDuration));
				seq.Insert(0, rect.DORotate(new Vector3(0, 0, obj.transform.rotation.eulerAngles.z + Randomer.Base.NextFloat(-180, +180)), trueDuration));
				// position
				seq.Append(rect.DOAnchorPos(new Vector2(offset.localPosition.x + (float)i / (children.Length - 1) * dividePower, offset.localPosition.y), secondPhaseDuration));
				// rotation:
				seq.Insert(trueDuration, obj.transform.DORotate(new Vector3(0, 0, 0), secondPhaseDuration));
				seq.SetLink(obj);
				

			}

		}
	}
}