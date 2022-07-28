using System;
using Cyberultimate.Unity;
using DG.Tweening;
using ParticleAttractor;
using UnityEngine;
namespace LetterBattle
{
    public class LaserEffectSpawnerOnLetterDestroyed : MonoBehaviour
    {
        [SerializeField] private ParticleAttractorLinear attractorParticlePrefab = null;
        [SerializeField] private Transform particleTarget = null;
    
        private void Awake()
        {
            LEvents.Base.OnLetterHpLost.Raw += OnSuccess;
        }

		protected void Start()
		{
            particleTarget = GameObject.FindGameObjectWithTag("Attractor").transform;
		}

		private void OnDestroy()
        {
            LEvents.Base.OnLetterHpLost.Raw -= OnSuccess;
        }
        private void OnSuccess(object sender, ActionLetter args)
        {

            if (args.DeathReason == ActionLetter.DeathType.KilledOnPlayer) return;
            ParticleAttractorLinear attractor = Instantiate(attractorParticlePrefab, args.transform.position, Quaternion.identity);
            attractor.Target = particleTarget;
            LaserManager.Spawn(args.CurrentAvailablePlanet.Center.Get2DPos(), args.transform.Get2DPos(), null, keepTrack: args.CurrentAvailablePlanet.Center);
        }

    }
}