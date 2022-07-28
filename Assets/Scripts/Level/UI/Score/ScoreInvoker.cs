using System;
using Cyberultimate.Unity;
using UnityEngine;
namespace LetterBattle
{
    public class ScoreInvoker: MonoSingleton<ScoreInvoker>
    {
        protected override void Awake()
        {
            base.Awake();
            LEvents.Base.OnLetterDestroyedByPlayerInteraction.Raw += OnLetterDestroyed;
            LEvents.Base.OnTargetNotFound.Raw += OnTargetNotFound;

        }
        private void OnTargetNotFound(object sender, char e)
        {
            ScoreSystem.Current.AddScoreWithComboRespect(GameAsset.Current.PunishForNotFound,"Not found");
        }
        private void OnDestroy()
        {
            LEvents.Base.OnLetterDestroyedByPlayerInteraction.Raw -= OnLetterDestroyed;
            LEvents.Base.OnTargetNotFound.Raw -= OnTargetNotFound;

        }
        private void OnLetterDestroyed(object sender, LetterActionArgsOnDeath args)
        {
            if (args.DeathType == ActionLetter.DeathType.KilledOnPlayer)
            { 
                ScoreSystem.Current.AddScoreWithComboRespect(GameAsset.Current.PunishForGettingHit,"Getting hit");
            }
            else
                ScoreSystem.Current.AddScoreWithComboRespect(GameAsset.Current.ScoreForShot,"Good shot");
        }
    }
}