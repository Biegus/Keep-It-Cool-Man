using System;
using DG.Tweening;
using LetterBattle.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace LetterBattle
{
    public class ComboText : MonoBehaviour
    {
    
        
        
        [SerializeField]
        private float fadeOutTime = 3;
        
        [SerializeField]
        private Ease animationEase = Ease.InOutQuint;
        
        [SerializeField]
        private float enterTime = 0.6f;
        
        [SerializeField]
        private float enterRotation = 100;
        
        private Text text;
        private bool inited = false;
        private int combo = 1;

        public void Init(int givenCombo, Vector2 pos)
        {
            if (inited) throw new InvalidOperationException("Init can be called only one time");
            inited = true;
            this.combo = givenCombo;
            this.transform.position = pos;
        }

        public void Start()
        {
            text = GetComponentInChildren<Text>();
            
            text.fontSize = 40 + (combo * 15);
            text.text = $"{combo}x";
            
            //Animations
            
            //Enter scale
            this.transform.localScale = new Vector2(0.01f,0.01f);
            this.transform.DOScale(Vector2.one, enterTime)
                .SetEase(animationEase)
                .SetLink(this.gameObject);
            
            //Enter rotate
            this.transform.eulerAngles -= new Vector3(0,0,enterRotation);
            this.transform.DoRotateAboutZ(enterRotation, enterTime)
                .SetEase(animationEase)
                .SetLink(this.gameObject)
                .OnComplete(() =>
                {
                    //Exit fade
                    text.DOFade(0f, fadeOutTime)
                        .OnComplete(() =>
                        {
                            Destroy(this.gameObject);
                        })
                        .SetLink(this.gameObject)
                        .SetEase(animationEase);
                });
                
                        
        }

    }
}