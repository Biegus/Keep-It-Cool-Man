using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace LetterBattle
{
    public class CutsceneController : MonoBehaviour
    {
        [SerializeField][Required()] private CutsceneManager manager;

        private float skipTimer;

        [SerializeField]
        private float bestTimeToSkip = 4f;

        [SerializeField]
        private Image circleFillImg = null;

        protected virtual void Update()
        {
            // SKIP sentence:
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
			{
                manager.Press();
            }


            // SKIP cutscene:
            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
			{
                skipTimer += Time.deltaTime;


                if (skipTimer > bestTimeToSkip / 4)
                {
                    circleFillImg.fillAmount = (skipTimer / 4) / (bestTimeToSkip / 4);
                }

                if (skipTimer > bestTimeToSkip)
				{
                    manager.Dispose();
                }
			}

            else
			{
                circleFillImg.fillAmount = 0;
                skipTimer = 0;

			}
        }
    }
}