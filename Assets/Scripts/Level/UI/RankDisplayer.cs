using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace LetterBattle
{
    public class RankDisplayer : MonoBehaviour
    {
        [SerializeField] private Text textEntity;
        [SerializeField] private Text descriptionEntity;
        private string formatText;
        private void Awake()
        {
            this.formatText = textEntity.text;
        }
        private void OnEnable()
        {
            var textData = ScoreSystem.Current.GetRank().GetTextData();
            textEntity.text = string.Format(formatText, $"<color=#{ColorUtility.ToHtmlStringRGB(textData.Color)}>{textData.Name}</color>");
            descriptionEntity.text = textData.Description;
        }
    }
}