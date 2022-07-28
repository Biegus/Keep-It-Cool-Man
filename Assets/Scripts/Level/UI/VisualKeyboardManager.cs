using Cyberultimate.Unity;
using DG.Tweening;
using LetterBattle.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LetterBattle
{
	public class VisualKeyboardManager : MonoBehaviour
	{

		[SerializeField]
		private TMP_Text[] keyButtons = null;
		[SerializeField]
		[InspectorButtonInformation("GetAllChildrenImages", "GetAllChildrenImages", InspectorButtonSize.Normal)]
		private InspectorButton<NoneType> b1 = new InspectorButton<NoneType>();
		[SerializeField]
		private Image[] images = null;


		[Header("Values")]
		[SerializeField] [Range(0, 1f)] private float pressedBackAlpha = 0.9f;
		[SerializeField] [NaughtyAttributes.MinValue(0.01f)] private float lerpingTime = 0.4f;
		[SerializeField] [NaughtyAttributes.MinValue(0.01f)] private float fullAppearanceTime = 0.2f;
		//that some symbols would be other than letters, it would require big array, with a lot of empty places. using dictionary doesn't cost that much
		//and  makes cleaner and easier to change in future. It's small dictionary anyway.

		private Tween[] tweens;
		private Color defaultTextColor;
		private float defaultImageAlpha;

		private Color originColor;
		private Color secondaryDefaultTextColor;

		

		private HashSet<char> savedHash;
		
	
		protected void Awake()
		{
			tweens= new Tween[GameAsset.Current.Keyboard.Keys.Length];
			if (keyButtons.Length == 0 || images.Length == 0) throw new InvalidOperationException("Either buttons or images were not set");

			LevelAsset cur = GameManager.CurrentLevel;
			savedHash = cur.GetSymbols();

			originColor = new Color(keyButtons[0].color.r, keyButtons[0].color.g, keyButtons[0].color.b, 0.2f);

			foreach (var text in keyButtons)
			{
				if (savedHash.Contains(char.ToLower(text.text[0])))
				{
					secondaryDefaultTextColor = new Color(originColor.r, originColor.g, originColor.b, 1);
					text.color = secondaryDefaultTextColor;
				}

				else
					text.color = originColor;
			}

			defaultTextColor = originColor;

			defaultImageAlpha = images[0].color.a;
		}

		protected void OnEnable()
		{
			LEvents.Base.OnLetterDestroyedByPlayerInteraction.Raw += OnLetterDestroyed;
			LEvents.Base.OnTargetNotFound.Raw += OnTargetNotFound;
		}

		protected void OnDisable()
		{
			LEvents.Base.OnLetterDestroyedByPlayerInteraction.Raw -= OnLetterDestroyed;
			LEvents.Base.OnTargetNotFound.Raw -= OnTargetNotFound;
		}
		private void OnTargetNotFound(object sender, char letter)
		{
			Flicker(letter, GameAsset.Current.Pallete.GetColor(ColorType.Danger));
		}
		private void OnLetterDestroyed(object sender, LetterActionArgsOnDeath args)
		{
			if (args.DeathType == ActionLetter.DeathType.KilledOnPlayer) return;
			Flicker(args.ActionLetter.Letter, GameAsset.Current.Pallete.GetColor(ColorType.Success));
		}
		private void Flicker(char letter, Color color)
		{
			int index = GameAsset.Current.Keyboard.GetIndex(letter);
			if (index == -1) return;
			if (index >= keyButtons.Length) return;
			TMP_Text txtEntity = keyButtons[index];
			Image imageEntity = images[index];
			txtEntity.color = color;
			imageEntity.color = new Clr(imageEntity.color, pressedBackAlpha);

			tweens[index].Kill(false);
			Sequence seq = DOTween.Sequence();
			seq.Insert(fullAppearanceTime, DOVirtual.Float(0, 1, lerpingTime, value =>
				{
					imageEntity.color = new Clr(imageEntity.color, Mathf.Lerp(pressedBackAlpha, defaultImageAlpha, value));

					if (savedHash.Contains(char.ToLower(letter)))
					{
						txtEntity.color = Color.Lerp(color, secondaryDefaultTextColor, value);
					}

					else
						txtEntity.color = Color.Lerp(color, defaultTextColor, value);

				}));
			seq.SetLink(this.gameObject);
			tweens[index] = seq;
		}
		[Conditional("UNITY_EDITOR")]
		private void GetAllChildrenImages()
		{
			images = this.transform.GetChildren().SelectMany(item => item.gameObject.GetComponentsInChildren<Image>()).ToArray();
		}




	}
}

