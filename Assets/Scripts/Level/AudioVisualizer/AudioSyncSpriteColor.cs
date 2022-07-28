using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSyncSpriteColor : AudioSyncColor
{
	private SpriteRenderer img = null;
	private Color restColor;

	protected void Start()
	{
		img = GetComponent<SpriteRenderer>();
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
