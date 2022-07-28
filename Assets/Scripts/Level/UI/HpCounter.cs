using DG.Tweening;
using System;
using Cyberultimate;
using LetterBattle.Utility;
using TMPro;
using UnityEngine;


namespace LetterBattle
{
	public class HpCounter : MonoBehaviour
	{
		[SerializeField][NaughtyAttributes.Required] TextMeshProUGUI textEntity;
		[SerializeField] private float flickingTime = 3;

		public Cyberultimate.LockValue<float> Hp { get; private set; }
		private string formatableText;
		private Tween smoothTextTween = null;
		private float lazyValue = 0;
		[SerializeField] private UnityEngine.Rendering.Universal.Light2D[] lighto = null;
		[SerializeField]
		private int startValue = 10;
		[SerializeField]
		private int endValue = 50;

		private Color startColor;

		private void Awake()
		{
			formatableText = textEntity.text;
			startColor = textEntity.color;

			Hp = LevelManager.Current.Hp;
			lazyValue = Hp.Value;

			Hp.OnValueChanged += OnValueTaken;
			Hp.OnValueChangedToMin += OnDeath;
			LEvents.Base.OnLevelLost.Raw += OnLevelLost;
			Refresh();
		}
		private void OnLevelLost(object sender, EventArgs e)
		{
			
			Destroy(this.gameObject);
		}
		private void OnDestroy()
		{
			if (Hp != null)
			{
				Hp.OnValueChanged -= OnValueTaken;
				Hp.OnValueChangedToMin -= OnDeath;
			};
			
			LEvents.Base.OnLevelLost.Raw -= OnLevelLost;
		}
		private void OnDeath(object sender, LockValue<float>.AnyValueChangedArgs e)
		{
			DoTweenHelper.Flicker(flickingTime, () =>
			{
				textEntity.color = this.textEntity.color == startColor ? GameAsset.Current.Pallete.GetColor(ColorType.Danger) : startColor;

			}).SetLink(this.gameObject)
				.OnComplete(() => { textEntity.color = GameAsset.Current.Pallete.GetColor(ColorType.Danger); });
		}
		private void OnValueTaken(object sender, EventArgs args)
		{
			Refresh();
		}
		private string GetStringForValue(float value)
		{
			return string.Format(formatableText, GetCelsiusHp(value));
		}

		private double GetCelsiusHp(float value)
		{
			return Math.Round(endValue - value / Hp.Max * (endValue - startValue));
		}

		private void Refresh()
		{
			//10 = 100 hp
			//50 = 0 hp
			smoothTextTween?.Kill(complete:false);
			smoothTextTween = DOVirtual.Float(lazyValue, Hp.Value, Math.Abs(lazyValue - Hp.Value) * 0.4f, (value) =>
			{
				if (lazyValue > Hp.Max)
					lazyValue = Hp.Max;
				
				if (Hp.Max>1000 && Hp.Value>500)
				{
					textEntity.text = string.Format(formatableText, "-∞");
					foreach (var light in lighto)
					{
						light.intensity = -0.1f;
					}
					
				}
				else
				{
					textEntity.text = GetStringForValue(value);
				
					if (lighto != null)
					{
						foreach (var light in lighto)
						{
							light.intensity = (1 - (lazyValue / Hp.Max))*0.15f;
						}
					}
					else
						Debug.LogWarning("Ligth was null");
					lazyValue = value;
				}
				
				
			}).SetLink(this.gameObject).SetUpdate(true).SetEase(Ease.Linear);

		}


	}
}