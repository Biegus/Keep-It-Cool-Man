using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace LetterBattle
{
	[Serializable]
	public class ActionText
	{
		[SerializeField]
		private UnityEvent action = null;
		public UnityEvent Action => action;

		[SerializeField]
		private Text text = null;
		public Text Text => text;
	}
}

