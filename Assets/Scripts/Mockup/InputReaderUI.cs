using System;
using System.Collections;
using System.Collections.Generic;
using Cyberultimate.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

namespace LetterBattle
{
	public class InputReaderUI : MonoBehaviour
	{
		private Tween tweenText = null;

		[SerializeField]
		private Transform[] center = null;
		public Transform[] Center { get; set; }

		[SerializeField]
		private float delay = 4f;

		[SerializeField]
		private GameObject particles = null;
		private Text relatedTarget;

		[SerializeField]
		private SerializedDictionary<char, ActionText> eventKeyDictionary = new SerializedDictionary<char, ActionText>();

		protected void Awake()
		{
			Center = center;
		}

		protected void Update()
		{
			foreach (char c in InputCharHelper.GetPressedKeys(true))
			{
				SingleInput(c);
			}
		}

		public void SingleInput(char c)
		{
			if (eventKeyDictionary.ContainsKey(c))
			{
				if (eventKeyDictionary[c].Text == null)
				{
					eventKeyDictionary[c].Action.Invoke();
					return;
				}
				if (eventKeyDictionary[c].Text.gameObject.activeSelf)
				{
					eventKeyDictionary[c].Action.Invoke();
					DestroyTarget(eventKeyDictionary[c].Text);
				}
			}
		}

		public void DestroyTarget(Text target)
		{
			target.transform.parent.GetComponent<Selectable>().Select();;
			foreach (var cen in Center)
			{
				//LaserManager.Spawn(cen.Get2DPos(), target.transform.Get2DPos());
			}

			ParticleHelper.Spawn(particles, target.transform.Get2DPos());
			tweenText?.Kill();
			if (relatedTarget != target && relatedTarget != null)
			{
				relatedTarget.color = new Clr(target.color, 1);
			}
			relatedTarget = target;
			
			target.color = new Clr(target.color, 0);
			tweenText = DOVirtual.DelayedCall(delay / 1.45f, 
				() => tweenText= target.DOFade(1, delay / 2f).SetLink(this.gameObject)).SetLink(this.gameObject);
		}
	}
}

