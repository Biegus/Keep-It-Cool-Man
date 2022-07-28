using System;
using Cyberultimate.Unity;
using DG.Tweening;
using UnityEngine;

namespace LetterBattle
{
    public static class AutoPilot
    {
        private static bool autoPilotEnabled = false;
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            if(autoPilotEnabled)
                InvertState();
        }
        public static void InvertState()
        {
            if(!autoPilotEnabled)
                LEvents.Base.OnLetterEnteredZone.Raw += AutoPilotOnEnteredZone;
            else
                LEvents.Base.OnLetterEnteredZone.Raw -= AutoPilotOnEnteredZone;
            autoPilotEnabled = !autoPilotEnabled;
        }
        private static void AutoPilotOnEnteredZone(object s, ActionLetter e)
        {
            DOVirtual.DelayedCall(0.1f, () => LEvents.Base.Input(e.Letter));
        }
    }
    [CommandContainer]
    public static class CheatCommand
    {
        [CyberCommand("cheat_game_auto_pilot")]
        public static void InvertAutoPilot()
        {
           AutoPilot.InvertState();
        }
       
        [CyberCommand("cheat_lv_set_hp")]
        public static void SetHp(string[] args)
        {
            int hp = int.Parse(args[0]);
            LevelManager.Current.Hp.SetMax(Math.Max(LevelManager.Current.Hp.Max,hp),"Command");
            LevelManager.Current.Hp.SetValue(hp,"command");
        }
    }
}