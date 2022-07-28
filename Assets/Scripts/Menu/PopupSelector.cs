using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LetterBattle
{
	public class PopupSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
	{
		[SerializeField]
		private Text levelText = null;

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			PopupManager.Current.SwitchPopup(true, int.Parse(levelText.text));
		}

		void ISelectHandler.OnSelect(BaseEventData eventData)
		{
			PopupManager.Current.SwitchPopup(true, int.Parse(levelText.text));
		}

		void IDeselectHandler.OnDeselect(BaseEventData eventData)
		{
			PopupManager.Current.SwitchPopup(false, int.Parse(levelText.text));
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			PopupManager.Current.SwitchPopup(false, int.Parse(levelText.text));
		}
	}
}

