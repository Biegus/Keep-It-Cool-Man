using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class AudioSyncScale : AudioSyncer
{
	[Header("Scale")]
	[SerializeField]
	private Vector2 beatScale = new Vector2(1, 1);

	[SerializeField]
	private bool scaleRelative = false;
	
	private Vector2 restScale;

	private Coroutine currCor = null;

	protected void Start()
	{
		restScale = this.transform.localScale;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if (isBeat)
		{
			return;
		}

		transform.localScale = Vector2.Lerp(transform.localScale, restScale, restSmoothTime * Time.deltaTime);
	}

	private IEnumerator MoveToScale(Vector2 target)
	{
		Vector2 curr = transform.localScale;
		Vector2 init = curr;
		float timer = 0;
		while (curr != target)
		{
			curr = Vector2.Lerp(init, target, timer / timeToBeat);
			timer += Time.deltaTime;

			transform.localScale = curr;

			yield return null;
		}

		isBeat = false;
	}

	protected override void OnBeat()
	{
		base.OnBeat();

		if (currCor != null)
		{
			StopCoroutine(currCor);
		}

		Vector2 finalScale = scaleRelative ? beatScale * restScale : beatScale;
		
		currCor = StartCoroutine(MoveToScale(finalScale));
	}
}
