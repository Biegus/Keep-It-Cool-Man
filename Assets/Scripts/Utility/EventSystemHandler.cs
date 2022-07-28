using Cyberultimate.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventSystemHandler : MonoSingleton<EventSystemHandler>
{
	private Button lastFocusedBtn = null;

	private static bool isMouse = false;
	private void SetKeyboardButton(Button btn)
	{
#if !UNITY_EDITOR
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
#endif

		if (btn == null)
		{
			return;
		}

		btn.Select();
		btn.OnSelect(null);
	}

	public void Focus(Button btn)
	{
		lastFocusedBtn = btn;
		if (!isMouse)
		{
			SetKeyboardButton(btn);
		}
		else
		{
			SetMouseButton();
		}
	}

	private void SetMouseButton()
	{
		#if !UNITY_EDITOR
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
#endif

		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.firstSelectedGameObject = null;
	}

	private void Update()
	{
		CheckForKeyboardPress();
		CheckForMousePress();
	}

	private void CheckForKeyboardPress()
	{
		if (Input.anyKeyDown && !(Input.GetMouseButtonDown(0)
	|| Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
		{
			// change to without mouse mode
			if (isMouse)
			{
				SetKeyboardButton(lastFocusedBtn);
				isMouse = false;
			}
		}
	}

	private void CheckForMousePress()
	{
		if (Input.GetMouseButtonDown(0)
	|| Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
		{
			// change to mouse mode
			if (!isMouse)
			{
				SetMouseButton();
				isMouse = true;
			}
		}


	}
}
