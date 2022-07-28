using System;
using Cyberultimate.Unity;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
namespace LetterBattle.Utility
{
    public class TriggeredTweenEffect : MonoBehaviour,ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
    {

       
        [SerializeField] private float time=1;
        [SerializeField] private Ease easeUp=Ease.Linear;
        [SerializeField] private Ease easeDown=Ease.Linear;
        private Tween tween;
        [SerializeField] private float scale;
       
        private void OnEnable()
        {
            if(EventSystem.current.currentSelectedGameObject==this.gameObject)
                BuildUpTween();
            tween?.Play();
        }
        private void OnDisable()
        {
            BuildDownTween();
            tween?.Pause();
        }
        
     
         void ISelectHandler.OnSelect(BaseEventData eventData)
         {
             BuildUpTween();
         }
         private void BuildUpTween()
         {

             tween?.Kill();
             tween = this.transform.DOScale(scale, time * (2 - this.transform.localScale.x))
                 .SetLink(this.gameObject).SetEase(easeUp).SetUpdate(true);
         }
         void IDeselectHandler.OnDeselect(BaseEventData eventData)
         {
             BuildDownTween();
         }
         private void BuildDownTween()
         {

             tween?.Kill();
             tween = this.transform.DOScale(1, time * (this.transform.localScale.x - 1) / (scale - 1)).SetEase(easeDown).SetUpdate(true);
         }
         void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
         {
             BuildUpTween();
         }
         void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
         {
             BuildDownTween();
         }
    }
}