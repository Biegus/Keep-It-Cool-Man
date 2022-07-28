using System;
using DG.Tweening;
using LetterBattle.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace LetterBattle
{
    //ui
    public class FlickingManager : MonoBehaviour, IValidator
    {
        [SerializeField] private TMP_FontAsset otherFont;
        [SerializeField] private bool onStart;
        [SerializeField][NaughtyAttributes.ShowIf(nameof(onStart))] private float startTime;

        private TMP_FontAsset baseFont;
        private TMP_Text text;
        private Graphic[] renders;
        private Tween tween;
        private Color baseColor;
        public static Tween TryFlick(GameObject obj, float time)
        {
            var flickingManager = obj.GetComponent<FlickingManager>();
            if (flickingManager) return flickingManager.Flick(time);
            else return null;

        }
        private void Awake()
        {
            Reload();      
        }
        public void Reload()
        {
            renders = this.GetComponentsInChildren<Graphic>();
            text = this.GetComponentInChildren<TMP_Text>();
            baseFont = text.font;
            if(renders.Length>0)
            baseColor = renders[0].color;
        }
        private void Start()
        {
            if (onStart)
                Flick(startTime);
        }
        private void ApplyFont(TMP_FontAsset font)
        {
            text.font = font;
        }
        public Tween Flick(float time)
        {
            tween?.Kill();
            return tween = DoTweenHelper.FlickerValue(time, ApplyFont, baseFont, otherFont).SetLink(this.gameObject);
        }
        public void Abort(bool let=false)
        {
            tween?.Kill(true);
            if (!let)
                ApplyFont(baseFont);
        }
        public ValidateResult Validate(string place)
        {
            if (this.GetComponentsInChildren<Graphic>() == null) return ValidateResult.One("No graphic component attached in or in any children");
            else return ValidateResult.Ok;
            
        }
    }
}