using System;
using Cyberultimate.Unity;
using LetterBattle.Utility;
using UnityEngine;
namespace LetterBattle
{
	public sealed class InputReader : MonoSingleton<InputReader>
	{

		public QueueBool DeafStatus { get; } = new QueueBool();

		 private PlayerMover mover;

		 protected override void Awake()
		 {
			 base.Awake();
		 }
		 private void Start()
		 {
			 mover = FindObjectOfType<PlayerMover>(); // bad;
		 }
		 private void FixedUpdate()
		{
			if (!mover||DeafStatus) return;
			if(Input.GetKey(KeyCode.UpArrow))
				mover.Push(Direction.Up);
			if(Input.GetKey(KeyCode.LeftArrow))
				mover.Push(Direction.Left);
			if(Input.GetKey(KeyCode.RightArrow))
				mover.Push(Direction.Right);
			if(Input.GetKey(KeyCode.DownArrow))
				mover.Push(Direction.Down);
                
		}
		private void Update()
		{
			if (Time.timeScale == 0) return;
			foreach (var key in InputCharHelper.GetPressedKeys(true))
			{
				if (GameAsset.Current.Keyboard.Has(key))
					CallLetter(key);
			}

		}
		private void CallLetter(char letter)
		{
			if (DeafStatus) return;
			LEvents.Base.Input(letter);
		}
	}
}