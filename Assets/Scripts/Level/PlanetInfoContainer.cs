using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LetterBattle
{
	public class PlanetInfoContainer : MonoBehaviour
	{
		[SerializeField]
		private VisualKeyboardManager keyboard = null;
		public VisualKeyboardManager Keyboard => keyboard;

		[SerializeField]
		private ScoreDisplayer scoreDisplay = null;
		public ScoreDisplayer ScoreDisplay => scoreDisplay;

		[SerializeField]
		private Transform particleAttractor = null;
		public Transform ParticleAttractor => particleAttractor;
	}
}

