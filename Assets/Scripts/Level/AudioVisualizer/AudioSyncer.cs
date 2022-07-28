using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSyncer : MonoBehaviour
{
	[SerializeField]
	private float bias = 5;

	[SerializeField]
	private float timeStep = 0.2f;

	[SerializeField]
	protected float timeToBeat = 0.05f;

	[SerializeField]
	protected float restSmoothTime = 2f;

	private float previousAudioValue;
	private float audioValue;
	private float timer;

	protected bool isBeat;

	protected void Update()
	{
		OnUpdate();
	}

	protected virtual void OnUpdate()
	{
		previousAudioValue = audioValue;
		audioValue = AudioSpectrum.SpectrumValue;
		if (previousAudioValue > bias && audioValue <= bias)
		{
			if (timer > timeStep)
			{
				OnBeat();
			}
		}

		if (previousAudioValue <= bias && audioValue > bias)
		{
			if (timer > timeStep)
			{
				OnBeat();
			}
		}

		timer += Time.deltaTime;
	}

	protected virtual void OnBeat()
	{
		timer = 0;
		isBeat = true;
	}
}
