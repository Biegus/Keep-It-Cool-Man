using System;
using LetterBattle;
using LetterBattle.Utility;
using UnityEngine;
namespace Experiments
{
    public class SymbolsTest : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log(GameAsset.Current.Levels[0].GetSymbols().BuildString());
        }
    }
}