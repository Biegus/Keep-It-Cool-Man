using System;
using DG.Tweening;
using UnityEngine;
namespace LetterBattle
{
    public class AutoDestructParticle : MonoBehaviour
    {
        private void Start()
        {
            var particle= this.GetComponent<ParticleSystem>();
            DOVirtual.DelayedCall(particle.main.startLifetimeMultiplier+particle.main.duration, () => Destroy(this.gameObject))
                .SetLink(this.gameObject);
        }
    }
}