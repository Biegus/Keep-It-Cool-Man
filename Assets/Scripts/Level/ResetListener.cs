using System;
using NaughtyAttributes;
using UnityEngine;

namespace LetterBattle
{
    public class ResetListener : MonoBehaviour
    {
        
        [SerializeField] private KeyCode resetKey = KeyCode.F1;

     
      
        private float holdingTime = 0;
        private void Update()
        {
           if(Input.GetKeyDown(KeyCode.F1))
               GameManager.RestartLevel();
        }
    }
}