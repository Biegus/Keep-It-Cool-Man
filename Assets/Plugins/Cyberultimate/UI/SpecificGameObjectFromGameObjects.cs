#nullable enable
using UnityEngine;

namespace Cyberultimate.Unity
{
#pragma warning disable IDE0044
	/// <summary>
	/// After invoking the <c>ButtonClick</c> method this script disables all sections in <c>allSections</c> array except the one that is specifed in method parameter.
	/// </summary>
	public class SpecificGameObjectFromGameObjects : MonoBehaviour
	{
		[SerializeField] private GameObject[] allSections = new GameObject[]{};
		public GameObject[] AllSections => allSections;

		private GameObject? lastSectionSelected = null;

		public void ButtonClick(GameObject specificSection)
		{
			foreach (GameObject item in allSections)
			{
				if (item != specificSection)
				{
					item.gameObject.SetActive(false);
				}
			}

			if (lastSectionSelected != specificSection)
			{
				specificSection.SetActive(true);
			}

			lastSectionSelected = specificSection;
		}
	}
}

