using System.Collections;
using UnityEngine;
namespace ParticleAttractor
{
	[RequireComponent(typeof(ParticleSystem))]
	public class ParticleAttractorLinear : MonoBehaviour 
	{
		private ParticleSystem ps;
		private ParticleSystem.Particle[] particles;

		[SerializeField]
		private Transform target;
		[SerializeField]
		private float speed = 5f;

		public Transform Target { get; set; }

		private int numParticlesAlive;

		protected void Awake()
		{
			Target = target;
		}

		protected void Start () 
		{
			ps = GetComponent<ParticleSystem>();
		}

		protected void Update () 
		{
			particles = new ParticleSystem.Particle[ps.main.maxParticles];
			numParticlesAlive = ps.GetParticles(particles);
			for (int i = 0; i < numParticlesAlive; i++) 
			{
				particles[i].position = Vector2.LerpUnclamped(particles[i].position, Target.position, speed * Time.deltaTime);
			}
			ps.SetParticles(particles, numParticlesAlive);
		}
	}

}
