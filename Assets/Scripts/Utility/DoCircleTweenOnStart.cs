using System;
using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace Utility
{
	public class DoCircleTweenOnStart : MonoBehaviour
	{
		[SerializeField] private CircleSettingHelper.SettingData data;
		[SerializeField] private Transform center;
		[SerializeField] private float firstPhaseDuration = 1;
		[SerializeField] private float diff = 1;
		[SerializeField] private float secondPhaseDuration = 1;
		[SerializeField] private float minDistance = 100;
		[SerializeField] private float maxDistance = 1000;
		[SerializeField] private float angleVariations = 10;
		
		private void Start()
		{
			var children = this.transform.GetChildren().Select(item => item.gameObject).ToArray();
			for (int i = 0; i < children.Length; i++)
			{
				var obj = children[i];
				RectTransform rect = obj.GetComponent<RectTransform>();
			
				Vector2 randomDir = (center.transform.position - obj.transform.position).normalized;
				randomDir = randomDir.GetRotated(Randomer.Base.NextFloat(-angleVariations, +angleVariations));
				var (pos, angle) = CircleSettingHelper.GetPosAndAngle(data, (float)i / (children.Length - 1));
				Sequence seq = DOTween.Sequence();
				
				var vec = randomDir * Randomer.Base.NextFloat(minDistance, maxDistance);
				float trueDuration = firstPhaseDuration + diff * i;
				seq.Append(rect.DOAnchorPos(rect.anchoredPosition -this.GetComponent<RectTransform>().anchoredPosition + vec, trueDuration));
				seq.Insert(0, rect.DORotate(new Vector3(0, 0, obj.transform.rotation.eulerAngles.z + Randomer.Base.NextFloat(-180, +180)), trueDuration));
				seq.Append(rect.DOAnchorPos(pos, secondPhaseDuration));
				seq.Insert(trueDuration, obj.transform.DORotate(new Vector3(0, 0, angle * Mathf.Rad2Deg), secondPhaseDuration));
				seq.SetLink(obj);
			



			}

		}
	}
}