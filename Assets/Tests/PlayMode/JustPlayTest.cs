using System.Collections;
using System.Collections.Generic;
using Cyberultimate.Unity;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LetterBattle;
using LetterBattle.Utility;
public class JustPlayTest
{
    [UnityTest]
    public IEnumerator JustPlayTestWithEnumeratorPasses()
    {
        
        yield return null;
        yield return null;
        GameManager.StartTheGame();
        yield return null;
        InputReader.Current.DeafStatus.RegisterObj(this);

        LEvents.Base.OnLetterEnteredZone.Raw += (s, e) =>
        {
            DOVirtual.DelayedCall(Randomer.Base.NextFloat(0.1f,0.3f), () => LEvents.Base.Input(e.Letter),false);
        };
        yield return null;
        LEvents.Base.OnLevelWon.Raw += (s, e) =>
        {
            GameManager.FinalizeLevel( -1, true);
            // DOVirtual.DelayedCall(0.1f, () => GameObject.FindGameObjectWithTag("InputRead").GetComponent<InputReaderUI>().SingleInput('N'));
        };
        while (GameManager.IsGameOn)
        {
            Time.timeScale = 5f;
            yield return null;
        }
    }
}
