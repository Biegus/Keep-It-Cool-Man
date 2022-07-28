using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LetterBattle
{
    public class LevelNameSetter : MonoBehaviour
    {
        [SerializeField]
        private Text levelNumber = null;

        [SerializeField]
        private Text levelName = null;

        protected void Start()
        {
            levelName.text = GameManager.CurrentLevel.ToString();
            levelNumber.text = string.Format(levelNumber.text, GameManager.CurrentLevelNumber);
        }
    }
}

