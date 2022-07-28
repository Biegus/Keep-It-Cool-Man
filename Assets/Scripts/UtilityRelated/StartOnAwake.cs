using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LetterBattle
{
    public class StartOnAwake : MonoBehaviour
    {
        private void Awake()
        {
           GameManager.StartTheGame();
        }
    }

}