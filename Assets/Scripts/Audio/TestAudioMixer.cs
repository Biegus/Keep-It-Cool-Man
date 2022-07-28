using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class TestAudioMixer : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mainMixer = null;

    [SerializeField]
    private float duration;

    private Sequence seq = null;

    private Dictionary<string, float> effectNameParameter = new Dictionary<string, float>();



    [SerializeField]
    private Ease transitionEase;

    public Tween Apply()
    {
        seq.Kill(true);
        seq = DOTween.Sequence();
        seq.Insert(0, mainMixer.DOSetFloat("cutOffFreq", 1850, duration));
        seq.Insert(0, mainMixer.DOSetFloat("octave", 5, duration));
        return seq.SetLink(this.gameObject).SetEase(transitionEase);
    }

    public Tween Reject()
	{
        seq.Kill(true);
        seq = DOTween.Sequence();
        seq.Insert(0, mainMixer.DOSetFloat("cutOffFreq", 10, duration));
        seq.Insert(0, mainMixer.DOSetFloat("octave", 4.2f, duration));
        return seq.SetLink(this.gameObject).SetEase(transitionEase);
    }
}
