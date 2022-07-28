using DG.Tweening;
using System;
using System.Linq;
using Cyberultimate.Unity;
using UnityEngine;


namespace LetterBattle
{
	public class Planet : MonoBehaviour
	{
		public enum Type
		{
			Earth,
			Moon,
			Other
		}

		[SerializeField] private GameObject particle;
		[SerializeField] private float explosionDelay = 3;
		[SerializeField] private SpriteRenderer spriteRen = null;
		[SerializeField] private Transform center;
		[SerializeField] private PlanetInfoContainer infoUI = null;
		[SerializeField] private CanvasGroup infoUICanvas = null;
		[SerializeField] private CanvasGroup hpUICanvas = null;

		
		public GameObject Zone { get; private set; }
		public GameObject Circle { get; private set; }
		
		public PlanetInfoContainer InfoUI => infoUI;
		public CanvasGroup InfoUICanvas => infoUICanvas;
		public CanvasGroup HpUICanvas => hpUICanvas;
		[SerializeField]
		private UnityEngine.Rendering.Universal.Light2D lighto = null;
		[SerializeField]
		private Type planetType;

		public Type PlanetType => planetType;

		public static event Action OnPlanetDeath = delegate { }; 

		public SpriteRenderer SpriteRen => spriteRen;
		public Transform Center
		{
			get
			{
				if (center) return center;
				else return this.transform;
			}

		}
		private void Awake()
		{
			Zone = this.transform.GetChildren().FirstOrDefault(item => item.CompareTag("PlayerZone"))?.gameObject; //it's bad ik
			Circle = this.transform.GetChildren().FirstOrDefault(item => item.name == "Circle0")?.gameObject;
		}
		private void Start()
		{

		
			
			LevelManager.Current.Hp.OnValueChangedToMin += OnDeath;
			LEvents.Base.OnLevelWon.RawAction += DisableLight;
		}

		private void DisableLight()
		{
			if (lighto != null)
				lighto.enabled = false;
		}

		private void OnDestroy()
		{
			LEvents.Base.OnLevelWon.RawAction -= DisableLight;

			if (LevelManager.Current != null)
				LevelManager.Current.Hp.OnValueChangedToMin -= OnDeath;
		}

		private void OnDeath(object sender, Cyberultimate.LockValue<float>.AnyValueChangedArgs args)
		{
			this.tag = "Untagged";

			LevelManager.Current.SceneBlockerStatus.RegisterObj(this);

			DOVirtual.DelayedCall(explosionDelay + 0.2f, () =>
				   {
					   Destroy(this.gameObject);
					   OnPlanetDeath.Invoke();
					   ParticleHelper.Spawn(particle, this.transform.position);
					   LevelManager.Current.SceneBlockerStatus.Unregister(this);

				   }).SetLink(this.gameObject);
		}

	}
}