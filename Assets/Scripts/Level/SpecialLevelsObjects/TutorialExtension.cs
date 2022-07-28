using System;
using System.Collections.Generic;
using Cyberultimate;
using DG.Tweening;
using LetterBattle;
using UnityEngine;
using LetterBattle.Utility;
namespace LetterBattle
{
	public class TutorialExtension : MonoBehaviour
	{

		private float scale = 1;
		private Dictionary<ActionLetter, InfoBox> data = new Dictionary<ActionLetter, InfoBox >();

		private Tween tween;
		private InfoBox infoBox = null;
		private float lazyTime = 1;
		private void Awake()
		{
			LEvents.Base.OnLetterEnteredZone.Raw += OnLetterEnteredZone;
			LEvents.Base.OnLetterDestroyed.Raw += OnLetterDestroyed_Raw;
			LevelManager.Current.Hp.OnValueChangedToMin += OnDeath;
		}
		private void GenerateTween()
		{
			tween?.Kill();
			if (scale - lazyTime == 0) { return;}
			
			tween = DOVirtual.Float(lazyTime, scale, Mathf.Abs(scale - lazyTime) * 0.5f, (v) =>
			{
				lazyTime = v;
				TimeScaling.Status.Register(this,lazyTime);
			}).SetLink(this.gameObject);
		}
		
		private void OnDeath(object sender, LockValue<float>.AnyValueChangedArgs e)
		{
			tween.Kill(false);
			TimeScaling.Status.Unregister(this);
			UnregisterAll();
		}

		private void OnDestroy()
		{
			tween.Kill(false);
			TimeScaling.Status.Unregister(this);
			UnregisterAll();
		}
		private void UnregisterAll()
		{
			if(LevelManager.Current)
				LevelManager.Current.Hp.OnValueChangedToMin -= OnDeath;
			LEvents.Base.OnLetterEnteredZone.Raw -= OnLetterEnteredZone;
			LEvents.Base.OnLetterDestroyed.Raw -= OnLetterDestroyed_Raw;
			foreach (var identity in data)
			{
				UnregisterIdentity(identity.Key, true);
			}
		}
		private void OnLetterDestroyed_Raw(object sender, ActionLetter e)
		{
			UnregisterIdentity(e);
		}
		private void OnLetterEnteredZone(object sender,ActionLetter args)
		{
			var letterToDestroy = args.Letter;
			infoBox = InfoBoxSpawner.Current.Spawn(args.transform.position, 
				$"Press <color={GameAsset.Current.Pallete.GetColorHex(ColorType.Danger)}>{letterToDestroy}</color>", 
				Color.white, 1);
			RegisterSlowMotionAsNewIdentity(args,infoBox);
		}
		private void UnregisterIdentity(ActionLetter actionLetter, bool dontRemove=false)
		{
			
			if (!dontRemove)
				data.Remove(actionLetter);
			if (data.Count == 0)
			{
				scale = 1;
				GenerateTween();
			}
		}
		private void RegisterSlowMotionAsNewIdentity(ActionLetter acitonLetter, InfoBox box)
		{
			if (data.ContainsKey(acitonLetter)) return;
			object identity = new object();
			data.Add(acitonLetter, box);
			scale = 0.25f;
			GenerateTween();
			
		}
	}
}