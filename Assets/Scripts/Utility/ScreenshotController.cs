using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotController : MonoBehaviour
{
	protected void Update()
	{
		if (Input.GetKeyDown(KeyCode.F12))
		{
			string time = System.DateTime.UtcNow.ToLocalTime().ToString("HH-mm-ss");
			string target = "Screenshots";
			if (!Directory.Exists(target))
			{
				Directory.CreateDirectory(target);
			}

			time = $"{Directory.GetCurrentDirectory()}/Screenshots/Screenshot{time}.png";
			ScreenCapture.CaptureScreenshot(time, 2);
		}
	}
}
