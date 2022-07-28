using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using LetterBattle.Utility;

namespace LetterBattle
{
	public class MenuPolyAnimation : MonoBehaviour
	{
		[SerializeField]
		private Image mainDeco = null;

		[SerializeField]
		private Image[] otherDecos = null;

		[SerializeField]
		private Image record = null;

		[SerializeField]
		protected float duration = 10f;

		[SerializeField]
		protected float durationRecord = 2f;

		protected void Awake()
		{
			DoRotate(mainDeco.transform, duration);
			
			DoRotate(record.transform, durationRecord);

			foreach (var item in otherDecos)
			{
				DoRotate(item.transform, duration * 0.4f);
			}
		}

		private Tween DoRotate(Transform trans, float duration)
		{
			return trans.DoRotateAboutZ(360, duration).SetLoops(-1).SetEase(Ease.Linear).SetLink(this.gameObject);
		}
	}
}

