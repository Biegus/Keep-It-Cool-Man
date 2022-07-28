using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using Cyberultimate;
using Cyberultimate.Unity;

namespace LetterBattle
{
	public class PostprocessingManager : MonoBehaviour
	{
		[SerializeField]
		private Volume volume = null;

		private Bloom bloom = null;
		private Vignette vignette = null;
		private ColorCurves adjustment = null;

		private float startScatter;
		private float startIntensityVignette;
		private float startSaturation;

		private Tween letterInput = null;
		private Vector3 savedCamRotation;

		protected virtual void Awake()
		{
			
			CallableEventMarker.Start(this);

			LEvents.Base.OnTargetNotFound.Register(OnWrongLetterInputed);
			LEvents.Base.OnLetterDestroyedByPlayerInteraction.Register(OnLetterDestroyed);
			LEvents.Base.OnComboChanged.Register(OnComboChanged);
			LEvents.Base.OnLevelLost.Register(EndLevelSetup);
			LEvents.Base.OnLevelWon.Register(EndLevelSetup);
			
			CallableEventMarker.EndStart(this);

		}

		protected void Start()
		{
			savedCamRotation = Camera.main.transform.rotation.eulerAngles;
			volume.profile.TryGet(out bloom);
			volume.profile.TryGet(out vignette);
			volume.profile.TryGet(out adjustment);

			startScatter = bloom.scatter.value;
			startIntensityVignette = vignette.intensity.value;
			startSaturation = adjustment.satVsSat.value[0].value;

			LevelManager.Current.Hp.OnValueChanged += OnHpChanged;
		}

		private void OnLetterDestroyed(object sender,LetterActionArgsOnDeath letterArgsDeath)
		{
			if (letterArgsDeath.DeathType == ActionLetter.DeathType.KilledByPlayer)
			{
				Sequence seq = DOTween.Sequence();
				seq.Insert(0, CameraHelper.Current.MainCamera.transform.DOLookAt(letterArgsDeath.ActionLetter.transform.position / 4.5f, 1.5f));
				seq.Insert(1.5f, CameraHelper.Current.MainCamera.transform.DORotate(savedCamRotation, 6));
				seq.SetLink(this.gameObject).SetEase(Ease.OutBack).SetUpdate(true);
			}
		}

		private void OnComboChanged(object sender, LEvents.ComboChangeArgs e)
		{
			ChangeSaturationTo(startSaturation + (e.CurrentCombo * 0.012f));
		}

		private void ChangeSaturationTo(float value)
		{
			Keyframe t = adjustment.satVsSat.value[0];
			t.value = value;
			t.time = 0;
			adjustment.satVsSat.value.MoveKey(0, t);
		}

		private void OnWrongLetterInputed()
		{
			// why do i even have to do this, wtf
			if (this == null)
			{
				return;
			}

			letterInput?.Kill(false);

			letterInput = LetterInput(bloom.scatter.value + 0.1f).OnComplete(() =>
			{
				LetterInput(startScatter);
			}).SetLink(this.gameObject);
		}

		private Tween LetterInput(float value)
		{
			return DOVirtual.Float(bloom.scatter.value, value, 0.6f, (f) =>
			{
				bloom.scatter.value = f;
			});
		}

		protected void OnDestroy()
		{
			if (LevelManager.Current.Hp != null)
				LevelManager.Current.Hp.OnValueChanged -= OnHpChanged;


			CallableEventMarker.End(this);
		}

		private void EndLevelSetup()
		{
			DisableVignette();
		}

		private void DisableVignette()
		{
			vignette.intensity.value = startIntensityVignette;
			vignette.color.value = Color.white;
		}

		private void OnHpChanged(object sender, LockValue<float>.AnyValueChangedArgs args)
		{
			float percentHP = args.LockValue.Value / args.LockValue.Max;

			if (percentHP <= 0.6f)
			{
				vignette.color.value = Color.red;
				vignette.intensity.value = (1 - percentHP) * 0.625f;
			}

			else
			{
				DisableVignette();
			}
		}
	}
}

