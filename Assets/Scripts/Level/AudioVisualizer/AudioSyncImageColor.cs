using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AudioSyncImageColor : AudioSyncColor
{
	private Image img = null;
	private Color restColor;

	protected void Start()
	{
		img = GetComponent<Image>();
		restColor = img.color;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if (isBeat)
		{
			return;
		}

		img.color = Color.Lerp(img.color, restColor, restSmoothTime * Time.deltaTime);
	}

	protected override IEnumerator MoveToColor(Color target)
	{
		Color curr = img.color;
		Color init = curr;
		float timer = 0;

		while (curr != target)
		{
			curr = Color.Lerp(init, target, timer / timeToBeat);
			timer += Time.deltaTime;

			img.color = curr;

			yield return null;
		}

		isBeat = false;
	}
}
