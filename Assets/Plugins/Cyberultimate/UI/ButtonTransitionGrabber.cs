
using UnityEngine;
using UnityEngine.UI;

namespace Cyberultimate.Unity
{
	[ExecuteInEditMode]
	public class ButtonTransitionGrabber : MonoBehaviour
	{
		[SerializeField] private ButtonTransitionAsset asset;
		public ButtonTransitionAsset Asset => asset;
		private int lastRefresh = 0;

		private void Update()
		{
#if UNITY_EDITOR
			if (asset == null) return;
			if (lastRefresh != asset.RefreshCount)
			{
				Refresh();
			}
#endif
		}
		private void Awake()
		{
			Refresh();
		}

		private void Refresh()
		{
			if (asset == null) return;
			Button button = this.GetComponent<Button>();
			if (button == null)
			{
				Debug.LogWarning("No button");
				return;
			}
			button.colors = asset.Content;
			lastRefresh = asset.RefreshCount;

		}
		private void OnValidate()
		{
			Refresh();

		}
	}
}