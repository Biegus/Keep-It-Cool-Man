using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace LetterBattle
{
    public class SettingsAnimationHandler : MonoBehaviour
    {
        [SerializeField]
        private Ease tweenEaseIn = DG.Tweening.Ease.OutCirc;
        [SerializeField]
        private Ease tweenEaseOut = DG.Tweening.Ease.OutQuad;
        [SerializeField]
        private float durationIn = 0.3f;
        [SerializeField]
        private float durationOut = 0.5f;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            TransitionHandler.Current.DOMoveAndZoom(durationIn, DG.Tweening.Ease.OutCirc, 2, new Vector2(-2.35f, 0));
        }

        public void OnBackBtn()
        {
            TransitionHandler.Current.DOMoveAndZoom( durationOut, DG.Tweening.Ease.OutQuad, positionToMove: new Vector2(0.001f, 0.001f));
            gameObject.SetActive(false);
        }
    }
}

