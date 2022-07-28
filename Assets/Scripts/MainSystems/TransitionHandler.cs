using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cyberultimate;
using Cyberultimate.Unity;

namespace LetterBattle
{
	public class TransitionHandler : MonoSingleton<TransitionHandler>
	{
		private static Scene? SavedScene = null;

		[SerializeField]
		private float camUnzoom = 5f;

		[SerializeField]
		private float camUnzoomDuration = 3f;

		[SerializeField]
		private float camZoom = 0.8f;

		[SerializeField]
		private float camZoomDuration = 3f;

		[SerializeField]
		private float delayUntilFunctional = 1f;

		[SerializeField]
		private Ease outEase = Ease.OutQuad;

		[SerializeField]
		private Ease inEase = Ease.OutBounce;
        
		protected void Start()
		{
			if (SavedScene.HasValue)
			{
				TimeScaling.Status.Register(this, 0);
				OutTransition();
				DOVirtual.DelayedCall(delayUntilFunctional, () => TimeScaling.Status.Unregister(this)).SetLink(this.gameObject);
				SavedScene = null;
			}
		}

		protected void OnDestroy()
		{
			TimeScaling.Status.Unregister(this);
		}

		private Tween OutTransition()
		{
			Camera.main.orthographicSize = camZoom;
			return ManualLevelTransition(false);
		}

        public Tween DOMoveAndZoom(float duration, Ease tweenEase, float orthoCamSizeToZoom = 5, Vector2 positionToMove = default)
		{
			Sequence seq = DOTween.Sequence();
			seq.Insert(0, CameraHelper.Current.MainCamera.DOOrthoSize(orthoCamSizeToZoom, duration));
			if (positionToMove != default)
            {
				float uniqueZ = CameraHelper.Current.MainCamera.transform.position.z;
				seq.Insert(0, CameraHelper.Current.MainCamera.transform.DOLocalMove(new Vector3(positionToMove.x, positionToMove.y, uniqueZ), duration));
			}
			seq.SetEase(tweenEase).SetLink(this.gameObject).SetUpdate(true);
			return seq;
		}

		public Tween ManualLevelTransition(bool isIn, Vector2 customPos = default)
		{
			return DOMoveAndZoom(isIn ? camZoomDuration : camUnzoomDuration, isIn ? inEase : outEase, isIn ? camZoom : camUnzoom, customPos);
		}

		public Tween DoTransition(bool silent = false, Vector2 customPos = default)
		{
			SavedScene = SceneManager.GetActiveScene();
			if (silent)
				return null;

			return ManualLevelTransition(true, customPos);
		}

	}
}

