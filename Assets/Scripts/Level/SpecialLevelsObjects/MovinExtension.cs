using System;
using DG.Tweening;
using UnityEngine;
namespace LetterBattle
{
    public class MovinExtension : MonoBehaviour
    {
        [SerializeField] private float dmg=0.2f;
        [SerializeField] private float delay=0.5f;
        [SerializeField] private float forLeter = 2;

        private Tween tween;
        private void Start()
        {
            tween= DOVirtual.DelayedCall(delay, () =>
            {
                LevelManager.Current.Hp.TakeValue(dmg, "time");
            }).SetLoops(-1).SetLink(this.gameObject);
            LEvents.Base.OnLetterDestroyedByPlayerInteraction.Raw += OnLetterDestroyed;
        }
        private void Update()
        {
            if (LevelManager.Current.LevelStatus != LevelManager.Status.Spawning)
            {
               
                Destroy(this);
            }
        }
        private void OnLetterDestroyed(object sender, LetterActionArgsOnDeath e)
        {
          LevelManager.Current.Hp.GiveValue(forLeter);
        }
        private void OnDestroy()
        {
            LEvents.Base.OnLetterDestroyedByPlayerInteraction.Raw -= OnLetterDestroyed;
            tween?.Kill();
            
          
        }

    }
}