using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSpectrum : MonoBehaviour
{
	private float[] audioSpectrum = null;

	public static float SpectrumValue { get; private set; }

	[SerializeField]
	private AudioSource currSource = null;

	protected void Start()
	{
		audioSpectrum = new float[128];
	}

	protected void Update()
	{
		if (currSource != null)
		{
			currSource.GetSpectrumData(audioSpectrum, 0, FFTWindow.Hamming);
		}

		else
		{
			AudioListener.GetSpectrumData(audioSpectrum, 0, FFTWindow.Hamming);
		}

		if (audioSpectrum != null && audioSpectrum.Length > 0)
		{
			SpectrumValue = audioSpectrum[0] * 100;
		}
	}
}
