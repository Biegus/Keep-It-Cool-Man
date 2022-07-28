using Cyberultimate.Unity;
using System.Collections.Generic;
using UnityEngine;
namespace LetterBattle
{
	public class CameraHelper : MonoSingleton<CameraHelper>
	{
		[SerializeField] private Camera mainCamera;

		private Vector2 basePos;
		private CyberCoroutine shaking = null;

		public Vector2 CameraSize { get; private set; }

		public const float CamOrthographicSize = 5f;
		public Camera MainCamera => mainCamera;
		protected override void Awake()
		{
			base.Awake();
			basePos = mainCamera.transform.Get2DPos();
			float h = 2 * CamOrthographicSize;
			float w = h * mainCamera.aspect;
			CameraSize = new Vector2(w, h);
		}

		public CyberCoroutine ShakeScreen(int shakingAmount = 10, float power = 0.015f, float delay = 0.01f)
		{
			shaking?.Stop();
			return shaking = Effects.ShakeObj(mainCamera.gameObject, basePos, shakingAmount, power, delay, this);
		}
		
	}
}