using Cyberultimate.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AudioSyncColor : AudioSyncer
{
	[SerializeField]
	private Color[] beatColors = null;

	private Coroutine currCor = null;

	protected override void OnBeat()
	{
		base.OnBeat();

		Color c = Randomer.Base.NextRandomElement(beatColors);

		if (currCor != null)
		{
			StopCoroutine(currCor);
		}

		currCor = StartCoroutine(MoveToColor(c));
	}

	protected abstract IEnumerator MoveToColor(Color target);
}
