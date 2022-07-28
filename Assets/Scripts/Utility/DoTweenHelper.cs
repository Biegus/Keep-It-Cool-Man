using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace LetterBattle.Utility
{
	public static class DoTweenHelper
	{
		public static Tween DoRotateAboutZ(this Transform transform, float angle, float duration, Action action = null, float limit = 360)
		{
			return DoRotateAbout(transform, angle, duration, new Vector3(0, 0, 1), action, limit);
		}

		public static Tween DoRotateAbout(this Transform transform, float angle, float duration, Vector3 axis)
		{
			var euler = transform.eulerAngles;
			float baseAngle = Mathf.Max(axis.x * euler.x, axis.y * euler.y, axis.z * euler.z);
			return DOVirtual.Float(0, angle, duration, (value) =>
			{
				transform.localRotation = Quaternion.AngleAxis(baseAngle + value, axis);
			});
		}
		public static Tween Flicker(float flickingTime, Action flickAction)
		{
			float got = 0;
			return DOVirtual.Float(0, 1, flickingTime, value =>
			{

				float cost = 0.4f - value * value *0.4f;
				if ((value - got) >= cost)
				{
					got += cost;
					flickAction();
				}
			});
		}
		public static Tween FlickerValue<T>(float flickingTime, Action<T> action, T bsColor, T endColor)
		{
			bool first = true;
			return Flicker(flickingTime, () =>
			{
				if (first) action(bsColor);
				else action(endColor);
				first = !first;
			}).OnComplete(()=>action(endColor));
		}
		

		public static Tween DoRotateAbout(this Transform transform, float angle, float duration, Vector3 axis, Action action, float limit)
		{
			var euler = transform.eulerAngles;
			float baseAngle = Mathf.Max(axis.x * euler.x, axis.y * euler.y, axis.z * euler.z);
			return DOVirtual.Float(0, angle, duration, (value) =>
			{
				transform.localRotation = Quaternion.AngleAxis(baseAngle + value, axis);
				if (value > limit)
				{
					action.Invoke();
				}
			});
		}
		public static Tween DoColor(this VolumeParameter<Color> par, Color endColor, float duration)
		{
			return DOTween.To(() => par.value, (v) => par.value = v, endColor, duration);
		}
		public static Tween DoUpdate(this GameObject obj, Action update)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));
			return DOVirtual.Float(0, 1, float.PositiveInfinity, (v) => update()).SetLink(obj);
		}

	
	}
}