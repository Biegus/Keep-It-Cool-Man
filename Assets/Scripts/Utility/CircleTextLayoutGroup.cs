using Cyberultimate.Unity;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
namespace LetterBattle.Utility
{
	public class CircleTextLayoutGroup : UIBehaviour
	{
		[InspectorButtonInformation("Refresh", "Refresh", InspectorButtonSize.Normal)]
		[SerializeField] private InspectorButton<NoneType> b1 = new InspectorButton<NoneType>();

		[SerializeField] private float radius = 0;
		[SerializeField] [Range(0, Mathf.PI)] private float sides = Mathf.PI / 2;
		[SerializeField] private float startPoint = 0;
		[SerializeField] private float baseRotation = 0;
		[SerializeField] private bool dontRotate = false;
		private void OnTransformChildrenChanged()
		{
			Refresh();
		}
		protected override void OnCanvasHierarchyChanged()
		{
			base.OnCanvasHierarchyChanged();
			Refresh();
		}
		protected override void OnEnable()
		{
			base.OnEnable();
			Refresh();
		}
		private void Refresh()
		{
			var childArray = this.transform.GetChildren().ToArray();
			for (int i = 0; i < childArray.Length; i++)
			{
				float len = childArray.Length - 1;
				var (pos, angle) = CircleSettingHelper.GetPosAndAngle(new CircleSettingHelper.SettingData(sides, radius), i / len, startPoint);
				angle += baseRotation;
				childArray[i].localPosition = pos;
				if (!dontRotate)
					childArray[i].transform.rotation = Quaternion.Euler(0, 0, (angle) / Mathf.PI * 180);
			}
		}
#if UNITY_EDITOR
		protected override void OnValidate()
		{
			base.OnValidate();
			Refresh();
		}
#endif
		protected override void OnDidApplyAnimationProperties()
		{
			Refresh();
		}
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			Refresh();
		}

	}
}