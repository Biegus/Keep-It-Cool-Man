using System;
using UnityEngine;

namespace LetterBattle
{
    public class DoubleHpBar : MonoBehaviour
    {
        [SerializeField]
        private Transform firstPlanet;

        [SerializeField]
        private Transform secondPlanet;

        private Vector2 savedPos;

        [SerializeField]
        private float freqMultiply = 0.9f;

        private void Start()
        {
            savedPos = this.transform.position;
            LEvents.Base.OnLevelWon.Raw += DisableOnWon;
			Planet.OnPlanetDeath += Planet_OnPlanetDeath;
        }

		private void Planet_OnPlanetDeath()
		{
            Destroy(this.gameObject);
		}

		private void OnDestroy()
        {
            LEvents.Base.OnLevelWon.Raw -= DisableOnWon;
            Planet.OnPlanetDeath -= Planet_OnPlanetDeath;
        }

		protected void Update()
		{
            if (firstPlanet == null || secondPlanet == null)
			{
                return;
			}

            SetToCenter(firstPlanet, secondPlanet);
		}

        private void SetToCenter(Transform startPos, Transform endPos)
		{
            Vector2 centerPos = new Vector2(transform.position.x, startPos.position.y + endPos.position.y) / 2;

            centerPos = new Vector2(centerPos.x, centerPos.y + savedPos.y);

            transform.position = new Vector2(transform.position.x, (centerPos.y * startPos.localScale.y * freqMultiply) + 
                (savedPos.y - centerPos.y * freqMultiply));
        }

		private void DisableOnWon(object sender, EventArgs args)
        {
            // wtf, need to change this:
            this.gameObject.SetActive(false);
        }
    }
}