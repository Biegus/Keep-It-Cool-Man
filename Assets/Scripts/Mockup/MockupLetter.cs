using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LetterBattle
{
	public class MockupLetter : MonoBehaviour
	{
		public MockupHandler Mockup { get; set; } = null;

		public DirectionMover DirMov { get; set; }

		protected void Awake()
		{
			DirMov = this.GetComponent<DirectionMover>();

			Invoke("Dstr", 9f);
		}

		private void Dstr()
		{
			MockupHandler.Current.RemoveLetter(this);
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.CompareTag("PlayerZone"))
			{
				MockupHandler.Current.AddLetter(this);
				return;
			}

			if (collision.CompareTag("Target"))
			{
				MockupHandler.Current.RemoveLetter(this);
			}
		}
	}
}

